using Newtonsoft.Json;

namespace Microscope.Net.DataModel
{
    public class CommentSubmit
    {
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }

        [JsonProperty(PropertyName = "postId")]
        public string PostId { get; set; }

        public CommentSubmit(string body, string postId)
        {
            this.Body = body;
            this.PostId = postId;
        }
    }
}
