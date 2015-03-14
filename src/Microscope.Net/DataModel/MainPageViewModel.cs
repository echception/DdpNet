using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Microscope.Net.DataModel
{
    using DdpNet;

    public class MainPageViewModel : INotifyPropertyChanged
    {
        private bool showLoadMore;

        public DdpFilteredCollection<Post> Posts { get; private set; }

        public MeteorClient Client { get; private set; }

        public bool ShowLoadMore
        {
            get { return this.showLoadMore; }
            internal set
            {
                this.showLoadMore = value;
                this.OnPropertyChanged();
            }
        }

        public MainPageViewModel(MeteorClient meteorClient, DdpCollection<Post> posts, bool showLoadMore)
        {
            this.Posts = posts.Filter(sortFilter: (post1, post2) => post2.Submitted.DateTime.CompareTo(post1.Submitted.DateTime));

            this.ShowLoadMore = showLoadMore;

            this.Client = meteorClient;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
