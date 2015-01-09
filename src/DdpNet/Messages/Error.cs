namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Error
    {
        [JsonProperty(PropertyName = "error")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        [JsonProperty(PropertyName = "details")]
        public string Details { get; set; }
    }
}
