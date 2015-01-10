namespace DdpNet.Messages
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class Added : BaseMessage
    {
        [JsonProperty(PropertyName = "collection")]
        public string Collection { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public JObject Fields { get; set; }

        public Added() : base("added")
        {
            
        }
    }
}
