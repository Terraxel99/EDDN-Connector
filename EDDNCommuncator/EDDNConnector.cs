namespace EDDNCommunicator
{
    using NetMQ;
    using NetMQ.Sockets;
    using NetMQ.Core.Utils;

    using Constants;

    using System;
    using System.Threading;
    using Ionic.Zlib;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using EDDNCommunicator.Models.Concretes;

    public class EDDNConnector : IDisposable
    {
        private readonly object socketLock = new object();

        /// <summary>
        /// The socket used for the connection.
        /// </summary>
        private readonly SubscriberSocket client;
        /// <summary>
        /// The utf8 encoding.
        /// </summary>
        private readonly UTF8Encoding encoding;
        /// <summary>
        /// The endpoint for connection.
        /// </summary>
        private readonly string endpoint;
        /// <summary>
        /// The timeout when receiving a message.
        /// </summary>
        private readonly TimeSpan timeout;

        /// <summary>
        /// Event triggered when event happens
        /// </summary>
        public event EventHandler<string> MessageReceived;

        /// <summary>
        /// Gets or sets a value indicating whether the client is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Creates an instance of an EDDNConnector with default domain and ports.
        /// </summary>
        /// <param name="socketMessageTimeout">The timeout when receiving a message (in seconds).</param>
        public EDDNConnector(double socketMessageTimeout = EDDNConstants.Connection.DefaultTimeout)
            : this(EDDNConstants.Connection.EDDNDomain, EDDNConstants.Connection.EDDNPort, socketMessageTimeout) { }

        /// <summary>
        /// Creates an instance of an EDDNConnector. This is to use if the domain or port has changed and your version of this library wouldn't be up to date.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="port">The port.</param>
        /// <param name="socketMessageTimeout">The timeout when receiving a message (in seconds).</param>
        public EDDNConnector(string domain, int port, double socketMessageTimeout = EDDNConstants.Connection.DefaultTimeout)
        {
            this.endpoint = $"{domain}:{port}";
            this.timeout = TimeSpan.FromSeconds(socketMessageTimeout);

            this.encoding = new UTF8Encoding();
            this.client = new SubscriberSocket();
        }

        /// <summary>
        /// Connects to EDDN.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Can't connect twice.
        /// or
        /// Client has been disposed.
        /// </exception>
        public void Connect()
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Can't connect twice.");

            if (this.client.IsDisposed)
                throw new InvalidOperationException("Client has been disposed.");
                
            this.client.Connect(this.endpoint);
            this.IsConnected = true;
            this.client.SubscribeToAnyTopic();

            this.MessageReceived += OnMessageReceived;

            new Thread(new ThreadStart(this.Listen))
                .Start();
        }

        /// <summary>
        /// Called when disposing class.
        /// </summary>
        public void Dispose()
        {
            lock (this.socketLock)
            {
                this.Disconnect();
                this.client.Close();
                this.client.Dispose();
            }
        }

        /// <summary>
        /// Closes the connection to EDDN.
        /// </summary>
        public void Close()
            => this.Dispose();

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            lock (this.socketLock)
            {
                this.MessageReceived -= this.OnMessageReceived;
                this.client.Disconnect(this.endpoint);
                this.IsConnected = false;
            }
        }

        /// <summary>
        /// Listens to messages from socket.
        /// </summary>
        private void Listen()
        {
            while (true)
            {
                lock (this.socketLock)
                {
                    if (this.client.IsDisposed)
                        break;

                    // If not message in the given timeout, loop will continue.
                    // We use that timeout to be able to make some operations on the client in other threads and avoid "dead" lock.
                    byte[] bytes;
                    bool isMessageAvailable = client.TryReceiveFrameBytes(this.timeout, out bytes);

                    if (!isMessageAvailable)
                        continue;

                    var uncompressed = ZlibStream.UncompressBuffer(bytes);

                    var result = this.encoding.GetString(uncompressed);
                    this.MessageReceived.Invoke(this, result);
                }
            }
        }

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnMessageReceived(object sender, string e)
        {
            var msg = JsonConvert.DeserializeObject<EDDNMessage>(e);
        }
    }
}
