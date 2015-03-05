namespace DdpNet.MessageHandlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Newtonsoft.Json.Linq;
    using Results;

    /// <summary>
    /// Handles all messages from the server that are sent in response to a client action
    /// </summary>
    internal class ReplyMessageHandler : IMessageHandler
    {
        /// <summary>
        /// All the messages to handle
        /// </summary>
        private static readonly string[] resultMessageTypes = new[] {"connected", "failed", "result", "ready", "updated", "nosub"};

        /// <summary>
        /// Processes the reply message
        /// </summary>
        /// <param name="client">The connection to send replies to the server</param>
        /// <param name="collectionManager">The CollectionManager</param>
        /// <param name="resultHandler">The ResultHandler</param>
        /// <param name="message">The message to process</param>
        /// <returns>Task to process the message</returns>
        public Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            JObject parsedObject = JObject.Parse(message);
            var result = new ReturnedObject((string)parsedObject["msg"], parsedObject, message);

            resultHandler.AddResult(result);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Determines if this class can handle a given message
        /// </summary>
        /// <param name="message">The message to see if we can handle</param>
        /// <returns>True if it can handle it, false otherwise</returns>
        public bool CanHandle(string message)
        {
            return resultMessageTypes.Contains(message, StringComparer.OrdinalIgnoreCase);
        }
    }
}
