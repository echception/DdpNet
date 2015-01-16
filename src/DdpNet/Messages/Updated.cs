namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Updated : BaseMessage
    {
        [JsonProperty(PropertyName = "methods")]
        public string[] Methods { get; set; }
        protected Updated() : base("updated")
        {
        }
    }
}
