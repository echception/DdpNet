namespace DdpNet.MessageHandlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Newtonsoft.Json.Linq;
    using Results;

    internal class ReplyMessageHandler : IMessageHandler
    {
        private static readonly string[] resultMessageTypes = new[] {"connected", "failed", "result", "ready", "updated"};

        public Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message)
        {
            JObject parsedObject = JObject.Parse(message);
            var result = new ReturnedObject((string)parsedObject["msg"], parsedObject, message);

            resultHandler.AddResult(result);

            return Task.FromResult(true);
        }

        public bool CanHandle(string message)
        {
            return resultMessageTypes.Contains(message, StringComparer.OrdinalIgnoreCase);
        }
    }
}
