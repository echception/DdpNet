using Newtonsoft.Json;

namespace Microscope.Net.DataModel
{
    public class PostReturn
    {
        [JsonProperty(PropertyName = "_id")]
        public string ID { get; set; }
    }
}
