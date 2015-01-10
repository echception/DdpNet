namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Messages;
    using Newtonsoft.Json;

    internal class ChangedHandler : BaseMessageHandler
    {
        public ChangedHandler() : base("changed")
        {
        }

        public override Task HandleMessage(DdpClient client, string message)
        {
            var changedMessage = JsonConvert.DeserializeObject<Changed>(message);
            client.CollectionManager.Changed(changedMessage);

            return Task.FromResult(true);
        }
    }
}
