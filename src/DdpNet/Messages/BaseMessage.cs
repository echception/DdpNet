namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class BaseMessage
    {
        [JsonProperty(PropertyName = "msg")]
        public string MessageType { get; set; }

        protected BaseMessage(string messageType)
        {
            this.MessageType = messageType;
        }
    }
}
