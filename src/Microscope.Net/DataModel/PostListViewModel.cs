using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Microscope.Net.DataModel
{
    using System;
    using DdpNet;

    public class PostListViewModel : BaseViewModel
    {
        private bool showLoadMore;

        public DdpFilteredCollection<Post> Posts { get; private set; }

        public bool ShowLoadMore
        {
            get { return this.showLoadMore; }
            internal set
            {
                this.showLoadMore = value;
                this.OnPropertyChanged();
            }
        }

        public PostListViewModel(MeteorClient meteorClient, DdpCollection<Post> posts, bool showLoadMore, Comparison<Post> sort )
        {
            this.Posts = posts.Filter(sortFilter: sort);

            this.ShowLoadMore = showLoadMore;
        }
    }
}
