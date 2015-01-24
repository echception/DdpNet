namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    internal class PingHandler : BaseMessageHandler
    {
        public PingHandler() : base("ping")
        {
        }

        public override Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            var pingMessage = JsonConvert.DeserializeObject<Ping>(message);
            var pongReply = new Pong() {ID = pingMessage.ID};

            return client.SendObject(pongReply);
        }
    }
}
