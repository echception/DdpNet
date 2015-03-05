namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Subscribe : BaseMessage
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "params")]
        public object[] Parameters { get; set; }

        public Subscribe(string id, string name, object[] parameters = null) : base("sub")
        {
            this.ID = id;
            this.Name = name;
            this.Parameters = parameters;
        }
    }
}
