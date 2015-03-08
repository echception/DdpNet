namespace Microscope.Net.DataModel
{
    using DdpNet;
    using Newtonsoft.Json;

    public class Post : DdpObject
    {
        private string title;
        private string url;
        private string userId;
        private string author;
        private Date submitted;
        private int commentsCount;
        private string[] upvoters;
        private int votes;

        [JsonProperty(PropertyName = "title")]
        public string Title
        {
            get { return this.title; }
            set
            {
                this.title = value;
                this.OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "url")]
        public string Url
        {
            get { return this.url; }
            set
            {
                this.url = value;
                this.OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "userId")]
        public string UserId
        {
            get { return this.userId; }
            set
            {
                this.userId = value;
                this.OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "author")]
        public string Author
        {
            get { return this.author; }
            set
            {
                this.author = value;
                this.OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "submitted")]
        public Date Submitted
        {
            get { return this.submitted; }
            set
            {
                this.submitted = value;
                this.OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "commentsCount")]
        public int CommentsCount
        {
            get { return this.commentsCount; }
            set
            {
                this.commentsCount = value;
                this.OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "upvoters")]
        public string[] Upvoters
        {
            get { return this.upvoters; }
            set
            {
                this.upvoters = value;
                this.OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "votes")]
        public int Votes
        {
            get { return this.votes; }
            set
            {
                this.votes = value;
                this.OnPropertyChanged();
            }
        }
    }
}
