namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    internal class ChangedHandler : BaseMessageHandler
    {
        public ChangedHandler() : base("changed")
        {
        }

        public override Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            var changedMessage = JsonConvert.DeserializeObject<Changed>(message);
            collectionManager.Changed(changedMessage);

            return Task.FromResult(true);
        }
    }
}
