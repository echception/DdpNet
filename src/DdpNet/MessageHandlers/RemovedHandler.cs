namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    /// <summary>
    /// Handles 'removed' messages from the server
    /// </summary>
    internal class RemovedHandler : BaseMessageHandler
    {
        /// <summary>
        /// Creates a new RemovedHandler
        /// </summary>
        public RemovedHandler() : base("removed")
        {
        }

        /// <summary>
        /// Processes the removed message
        /// </summary>
        /// <param name="client">The connection to send replies to the server</param>
        /// <param name="collectionManager">The CollectionManager</param>
        /// <param name="resultHandler">The ResultHandler</param>
        /// <param name="message">The message to process</param>
        /// <returns>Task to process the message</returns>
        public override Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            var removedMessage = JsonConvert.DeserializeObject<Removed>(message);
            collectionManager.Removed(removedMessage);

            return Task.FromResult(true);
        }
    }
}
