using System.Linq;

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

        protected override void Initialized()
        {
            App.Current.Client.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "User")
                {
                    this.OnPropertyChanged("CanUpvote");
                }
            };
        }

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
                this.OnPropertyChanged("CanUpvote");
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

        [JsonIgnore]
        public bool CanUpvote
        {
            get
            {
                if (App.Current.Client.User == null)
                {
                    return false;
                }

                var userId = App.Current.Client.User.ID;

                if (this.upvoters.Contains(userId))
                {
                    return false;
                }

                return true;
            }
        }

        [JsonIgnore]
        public bool OwnsPost
        {
            get
            {
                if (App.Current.Client.User != null)
                {
                    if (App.Current.Client.User.ID == this.userId)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
