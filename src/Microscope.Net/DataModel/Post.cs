namespace Microscope.Net.DataModel
{
    using DdpNet;
    using Newtonsoft.Json;

    public class Post : DdpObject
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "submitted")]
        public Date Submitted { get; set; }

        [JsonProperty(PropertyName = "commentsCount")]
        public int CommentsCount { get; set; }

        [JsonProperty(PropertyName = "upvoters")]
        public string[] Upvoters { get; set; }

        [JsonProperty(PropertyName = "votes")]
        public int Votes { get; set; }
    }
}
