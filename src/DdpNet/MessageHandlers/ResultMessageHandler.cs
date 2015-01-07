namespace DdpNet.MessageHandlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using Results;

    internal class ResultMessageHandler : IMessageHandler
    {
        private static readonly string[] resultMessageTypes = new[] {"connected"};

        public Task HandleMessage(DdpClient client, string message)
        {
            dynamic parsedObject = JObject.Parse(message);
            var result = new Result((string)parsedObject.msg, parsedObject, message);

            client.ResultHandler.AddResult(result);

            return Task.FromResult(true);
        }

        public bool CanHandle(string message)
        {
            return resultMessageTypes.Contains(message, StringComparer.OrdinalIgnoreCase);
        }
    }
}
