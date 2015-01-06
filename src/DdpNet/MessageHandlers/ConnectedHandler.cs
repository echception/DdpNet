namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Messages;
    using Newtonsoft.Json;

    internal class ConnectedHandler : BaseMessageHandler
    {
        public ConnectedHandler() : base("connected")
        {
        }

        public override Task HandleMessage(DdpClient client, string message)
        {
            var connected = JsonConvert.DeserializeObject<Connected>(message);

            client.SetSession(connected.Session);

            return Task.FromResult(true);
        }
    }
}
