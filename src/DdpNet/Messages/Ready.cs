namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Ready : BaseMessage
    {
        [JsonProperty(PropertyName = "subs")]
        public string[] SubscriptionsReady { get; set; }
        internal Ready() : base("ready")
        {
        }
    }
}
