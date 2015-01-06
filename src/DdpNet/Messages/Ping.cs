namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Ping : BaseMessage
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        public Ping() : base("ping")
        {
        }
    }
}
