namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;

    internal abstract class BaseMessageHandler : IMessageHandler
    {
        private readonly string messageHandled;

        protected BaseMessageHandler(string messageHandled)
        {
            this.messageHandled = messageHandled;
        }

        public abstract Task HandleMessage(DdpClient client, string message);

        public bool CanHandle(string message)
        {
            return string.Equals(this.messageHandled, message);
        }
    }
}
