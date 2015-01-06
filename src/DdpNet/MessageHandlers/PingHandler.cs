namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Messages;
    using Newtonsoft.Json;

    internal class PingHandler : BaseMessageHandler
    {
        public PingHandler() : base("ping")
        {
        }

        public override Task HandleMessage(DdpClient client, string message)
        {
            var pingMessage = JsonConvert.DeserializeObject<Ping>(message);
            var pongReply = new Pong() {ID = pingMessage.ID};

            return client.SendObject(pongReply);
        }
    }
}
