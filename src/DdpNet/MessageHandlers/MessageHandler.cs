namespace DdpNet.MessageHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    internal class MessageHandler
    {
        private List<IMessageHandler> handlers;

        public MessageHandler()
        {
            this.GetHandlers();
        }

        internal async Task HandleMessage(DdpClient client, string messageText)
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
                        await handler.HandleMessage(client, messageText);
                    }
                }
            }
        }

        private void GetHandlers()
        {
            this.handlers = new List<IMessageHandler>();

            this.handlers.Add(new PingHandler());
            this.handlers.Add(new ReplyMessageHandler());
            this.handlers.Add(new AddedHandler());
            this.handlers.Add(new ChangedHandler());
        }
    }
}
