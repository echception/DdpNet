namespace DdpNet.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class Changed : BaseMessage
    {
        [JsonProperty(PropertyName = "collection")]
        public string Collection { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public Dictionary<string, JToken> Fields { get; set; }

        [JsonProperty(PropertyName = "cleared")]
        public string[] Cleared { get; set; } 

        protected Changed() : base("changed")
        {
        }
    }
}
