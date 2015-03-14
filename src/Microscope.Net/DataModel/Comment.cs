using DdpNet;
using Newtonsoft.Json;

namespace Microscope.Net.DataModel
{
    public class Comment : DdpObject
    {
        private string postId;
        private string body;
        private string userId;
        private string author;
        private Date submitted;

        [JsonProperty(PropertyName = "postId")]
        public string PostId
        {
            get { return this.postId; }
            set { this.postId = value; this.OnPropertyChanged(); }
        }

        [JsonProperty(PropertyName = "body")]
        public string Body
        {
            get { return this.body; }
            set { this.body = value; this.OnPropertyChanged(); }
        }

        [JsonProperty(PropertyName = "userId")]
        public string UserId
        {
            get { return this.userId; }
            set { this.userId = value; this.OnPropertyChanged(); }
        }

        [JsonProperty(PropertyName = "author")]
        public string Author
        {
            get { return this.author; }
            set { this.author = value; this.OnPropertyChanged(); }
        }

        [JsonProperty(PropertyName = "submitted")]
        public Date Submitted
        {
            get { return this.submitted; }
            set { this.submitted = value; this.OnPropertyChanged(); }
        }
    }
}
