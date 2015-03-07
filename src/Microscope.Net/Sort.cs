namespace Microscope.Net
{
    using System.Dynamic;
    using Newtonsoft.Json;

    public class Sort
    {
        [JsonProperty(PropertyName = "submitted")]
        public int Submitted { get; set; }

        [JsonProperty(PropertyName = "_id")]
        public int ID { get; set; }
    }

    public class SubscribeParamters
    {
        [JsonProperty(PropertyName = "sort")]
        public Sort Sort { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public int Limit { get; set; }
    }
}
