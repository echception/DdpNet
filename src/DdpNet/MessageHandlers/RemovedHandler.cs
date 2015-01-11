namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Messages;
    using Newtonsoft.Json;

    internal class RemovedHandler : BaseMessageHandler
    {
        public RemovedHandler() : base("removed")
        {
        }

        public override Task HandleMessage(DdpClient client, string message)
        {
            var removedMessage = JsonConvert.DeserializeObject<Removed>(message);
            client.CollectionManager.Removed(removedMessage);

            return Task.FromResult(true);
        }
    }
}
