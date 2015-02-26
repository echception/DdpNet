namespace DdpNet.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class Method : BaseMessage
    {
        [JsonProperty(PropertyName = "method")]
        public string MethodName { get; set; }

        [JsonProperty(PropertyName = "params")]
        public List<object> Parameters { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        internal Method() : base("method")
        {

        }
    }
}
