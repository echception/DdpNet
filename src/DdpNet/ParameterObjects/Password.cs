namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    internal class Password
    {
        [JsonProperty(PropertyName = "digest")]
        public string Digest { get; set; }

        [JsonProperty(PropertyName = "algorithm")]
        public string Algorithm { get; set; }

        public Password(string digest, string algorithm)
        {
            this.Digest = digest;
            this.Algorithm = algorithm;
        }
    }
}
