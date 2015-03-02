namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Unsubscribe : BaseMessage
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        public Unsubscribe(string id) : base("unsub")
        {
            this.ID = id;
        }
    }
}
