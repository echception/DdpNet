namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class AddedHandler : BaseMessageHandler
    {
        public AddedHandler() : base("added")
        {
        }

        public override Task HandleMessage(DdpClient client, string message)
        {
            var parsedObject = JsonConvert.DeserializeObject<Added>(message);
            client.CollectionManager.Added(parsedObject);

            return Task.FromResult(true);
        }
    }
}
