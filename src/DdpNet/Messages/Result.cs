namespace DdpNet.Messages
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class Result : BaseMessage
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "result")]
        public JToken ResultObject { get; set; }

        internal Result() : base("result")
        {
        }
    }
}
