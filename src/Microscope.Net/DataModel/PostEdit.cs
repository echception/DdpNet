using Newtonsoft.Json;

namespace Microscope.Net.DataModel
{
    public class PostEdit
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        public PostEdit(string url, string title)
        {
            this.Url = url;
            this.Title = title;
        }
    }
}
