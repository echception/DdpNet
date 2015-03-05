namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    /// <summary>
    /// Handles 'ping' messages from the server
    /// </summary>
    internal class PingHandler : BaseMessageHandler
    {
        /// <summary>
        /// Creates a new PingHandler
        /// </summary>
        public PingHandler() : base("ping")
        {
        }

        /// <summary>
        /// Processes the ping message
        /// </summary>
        /// <param name="client">The connection to send replies to the server</param>
        /// <param name="collectionManager">The CollectionManager</param>
        /// <param name="resultHandler">The ResultHandler</param>
        /// <param name="message">The message to process</param>
        /// <returns>Task to process the message</returns>
        public override Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            var pingMessage = JsonConvert.DeserializeObject<Ping>(message);
            var pongReply = new Pong() {ID = pingMessage.ID};

            return client.SendObject(pongReply);
        }
    }
}
