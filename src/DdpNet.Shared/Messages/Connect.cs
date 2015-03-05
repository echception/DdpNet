namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Connect : BaseMessage
    {
        [JsonProperty(PropertyName = "session")]
        public string Session { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "support")]
        public string[] VersionsSupported { get; set; }

        public Connect() : base("connect")
        {
            
        }
    }
}
