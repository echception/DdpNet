namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Results;

    internal abstract class BaseMessageHandler : IMessageHandler
    {
        private readonly string messageHandled;

        protected BaseMessageHandler(string messageHandled)
        {
            this.messageHandled = messageHandled;
        }

        public abstract Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message);

        public bool CanHandle(string message)
        {
            return string.Equals(this.messageHandled, message);
        }
    }
}
