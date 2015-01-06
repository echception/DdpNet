namespace DdpNet.MessageHandlers
{
    using System;
    using System.Threading.Tasks;

    internal class ConsoleLogHandler : IMessageHandler
    {
        public Task HandleMessage(DdpClient client, string message)
        {
            Console.WriteLine(message);

            return Task.FromResult(true);
        }

        public bool CanHandle(string message)
        {
            return true;
        }
    }
}
