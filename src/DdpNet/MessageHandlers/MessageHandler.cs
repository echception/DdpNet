namespace DdpNet.MessageHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Newtonsoft.Json.Linq;
    using Results;

    internal class MessageHandler
    {
        private List<IMessageHandler> handlers;

        public MessageHandler()
        {
            this.GetHandlers();
        }

        public async Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string messageText)
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
                        await handler.HandleMessage(client, collectionManager, resultHandler, messageText);
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
            this.handlers.Add(new RemovedHandler());
        }
    }
}
