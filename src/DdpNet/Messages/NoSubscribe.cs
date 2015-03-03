namespace DdpNet.Messages
{
    using Newtonsoft.Json;
    using ReturnedObjects;

    internal class NoSubscribe : BaseMessage
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        protected NoSubscribe() : base("nosub")
        {
        }
    }
}
