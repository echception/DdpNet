namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Messages;
    using Newtonsoft.Json;
    using Results;

    /// <summary>
    /// Processes the 'changed' messages received from the server
    /// </summary>
    internal class ChangedHandler : BaseMessageHandler
    {
        /// <summary>
        /// Creates a new ChangedHandler
        /// </summary>
        public ChangedHandler() : base("changed")
        {
        }

        /// <summary>
        /// Processes the changed message
        /// </summary>
        /// <param name="client">The connection to send replies to the server</param>
        /// <param name="collectionManager">The CollectionManager</param>
        /// <param name="resultHandler">The ResultHandler</param>
        /// <param name="message">The message to process</param>
        /// <returns>Task to process the message</returns>
        public override Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            var changedMessage = JsonConvert.DeserializeObject<Changed>(message);
            collectionManager.Changed(changedMessage);

            return Task.FromResult(true);
        }
    }
}
