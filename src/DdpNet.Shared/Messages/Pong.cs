namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Pong : BaseMessage
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        public Pong() : base("pong")
        {
        }
    }
}
