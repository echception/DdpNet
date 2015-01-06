namespace DdpNet
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MessageHandlers;
    using Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class MessageHandler
    {
        private List<IMessageHandler> handlers;

        public MessageHandler()
        {
            this.GetHandlers();
        }

        internal void HandleMessage(DdpClient client, string messageText)
        {
            dynamic message = JObject.Parse(messageText);

            var msg = message.msg;

            if (msg != null)
            {
                var messageType = (string) msg;
                foreach (var handler in this.handlers)
                {
                    if (handler.CanHandle(messageType))
                    {
                        handler.HandleMessage(client, messageText);
                    }
                }
            }
        }

        private void GetHandlers()
        {
            this.handlers = new List<IMessageHandler>();

            this.handlers.Add(new ConnectedHandler());
            this.handlers.Add(new PingHandler());
        }
    }
}
