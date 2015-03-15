using Windows.ApplicationModel.Background;
using Newtonsoft.Json;

namespace Microscope.Net.DataModel
{
    public class PostSubmit
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        public PostSubmit(string title, string url)
        {
            this.Title = title;
            this.Url = url;
        }
    }
}
