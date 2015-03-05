namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    /// <summary>
    /// Processes 'added' messages received from the server
    /// </summary>
    internal class AddedHandler : BaseMessageHandler
    {
        /// <summary>
        /// Creates a new AddedHandler
        /// </summary>
        public AddedHandler() : base("added")
        {
        }

        /// <summary>
        /// Processes the added message
        /// </summary>
        /// <param name="client">The connection to send replies to the server</param>
        /// <param name="collectionManager">The CollectionManager</param>
        /// <param name="resultHandler">The ResultHandler</param>
        /// <param name="message">The message to process</param>
        /// <returns>Task to process the message</returns>
        public override Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            var parsedObject = JsonConvert.DeserializeObject<Added>(message);
            collectionManager.Added(parsedObject);

            return Task.FromResult(true);
        }
    }
}
