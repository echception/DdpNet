namespace DdpNet
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MessageHandlers;

    internal class MessageHandler
    {
        private List<IMessageHandler> handlers;

        public MessageHandler()
        {
            this.GetHandlers();
        }

        internal void HandleMessage(DdpClient client, string message)
        {
            foreach (var handler in this.handlers)
            {
                if (handler.CanHandle(message))
                {
                    handler.HandleMessage(client, message);
                }
            }
        }

        private void GetHandlers()
        {
            this.handlers = new List<IMessageHandler>();

            this.handlers.Add(new ConsoleLogHandler());
            this.handlers.Add(new ConnectedHandler());
        }
    }
}
