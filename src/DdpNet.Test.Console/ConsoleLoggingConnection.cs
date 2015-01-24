namespace DdpNet.Test.Console
{
    using System;
    using System.Threading.Tasks;
    using Connection;

    internal class ConsoleLoggingConnection : IWebSocketConnection
    {
        private IWebSocketConnection webSocketConnection;

        public ConsoleLoggingConnection(IWebSocketConnection connection)
        {
            this.webSocketConnection = connection;
        }

        public Task ConnectAsync()
        {
            return this.webSocketConnection.ConnectAsync();
        }

        public Task CloseAsync()
        {
            return this.CloseAsync();
        }

        public Task SendAsync(string text)
        {
            this.WriteMessage(text, ConsoleColor.Green);
            return this.webSocketConnection.SendAsync(text);
        }

        public async Task<string> ReceiveAsync()
        {
            var text = await this.webSocketConnection.ReceiveAsync();

            this.WriteMessage(text, ConsoleColor.White);

            return text;
        }

        private void WriteMessage(string message, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }
    }
}
