namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    internal class RemovedHandler : BaseMessageHandler
    {
        public RemovedHandler() : base("removed")
        {
        }

        public override Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            var removedMessage = JsonConvert.DeserializeObject<Removed>(message);
            collectionManager.Removed(removedMessage);

            return Task.FromResult(true);
        }
    }
}
