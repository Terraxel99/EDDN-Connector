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

    public class EDDNConnector : IDisposable
    {
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
        /// Event triggered when event happens
        /// </summary>
        public event EventHandler<string> MessageReceived;

        /// <summary>
        /// Creates an instance of an EDDNConnector with default domain and ports.
        /// </summary>
        public EDDNConnector()
            : this(EDDNConstants.Connection.EDDNDomain, EDDNConstants.Connection.EDDNPort) { }

        /// <summary>
        /// Creates an instance of an EDDNConnector. This is to use if the domain or port has changed and your version of this library wouldn't be up to date.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="port">The port.</param>
        public EDDNConnector(string domain, int port)
        {
            this.endpoint = $"{domain}:{port}";

            this.encoding = new UTF8Encoding();
            this.client = new SubscriberSocket();
        }

        /// <summary>
        /// Connects to EDDN.
        /// </summary>
        public void Connect()
        {
            this.client.Connect(this.endpoint);
            this.client.SubscribeToAnyTopic();

            new Thread(new ThreadStart(this.Listen))
                .Start();
        }

        /// <summary>
        /// Called when disposing class.
        /// </summary>
        public void Dispose()
        {
            this.client.Disconnect(this.endpoint);
            this.client.Close();
            this.client.Dispose();
        }

        /// <summary>
        /// Closes the connection to EDDN.
        /// </summary>
        public void Close()
            => this.Dispose();

        /// <summary>
        /// Listens to messages from socket.
        /// </summary>
        private void Listen()
        {
            while (!this.client.IsDisposed)
            {
                var bytes = client.ReceiveFrameBytes();
                var uncompressed = ZlibStream.UncompressBuffer(bytes);

                var result = this.encoding.GetString(uncompressed);
                this.MessageReceived.Invoke(this, result);
            }
        }
    }
}
