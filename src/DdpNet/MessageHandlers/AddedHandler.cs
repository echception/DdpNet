namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Results;

    internal class AddedHandler : BaseMessageHandler
    {
        public AddedHandler() : base("added")
        {
        }

        public override Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            var parsedObject = JsonConvert.DeserializeObject<Added>(message);
            collectionManager.Added(parsedObject);

            return Task.FromResult(true);
        }
    }
}
