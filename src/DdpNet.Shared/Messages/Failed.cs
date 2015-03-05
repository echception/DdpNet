namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Failed : BaseMessage
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        internal Failed() : base("failed")
        {
        }
    }
}
