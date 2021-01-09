namespace TestEDDN
{
    using EDDNCommunicator;
    using System;
    using System.Text;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var connector = new EDDNConnector();
            connector.MessageReceived += OnMessageReceived;
            connector.Connect();

            Thread.Sleep(4000);
            connector.Close();
        }

        private static void OnMessageReceived(object sender, string e)
        {
            Console.WriteLine(e);
        }
    }
}
