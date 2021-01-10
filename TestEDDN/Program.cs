namespace TestEDDN
{
    using EDDNCommunicator;

    using System;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            var connector = new EDDNConnector();
            connector.MessageReceived += OnMessageReceived;
            connector.Connect();
        }

        private static void OnMessageReceived(object sender, string e)
        {
            Console.WriteLine(e);
        }
    }
}
