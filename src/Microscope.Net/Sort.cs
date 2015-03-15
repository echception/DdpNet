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

    public class BestSort
    {
        [JsonProperty(PropertyName = "votes")]
        public int Votes { get; set; }

        [JsonProperty(PropertyName = "submitted")]
        public int Submitted { get; set; }

        [JsonProperty(PropertyName = "_id")]
        public int ID { get; set; }
    }

    public class SubscribeParamters
    {
        [JsonProperty(PropertyName = "sort")]
        public object Sort { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public int Limit { get; set; }
    }
}
