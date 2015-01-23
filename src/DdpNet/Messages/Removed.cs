namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Removed : BaseMessage
    {
        [JsonProperty(PropertyName = "collection")]
        public string Collection { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        internal Removed() : base("removed")
        {
        }
    }
}
