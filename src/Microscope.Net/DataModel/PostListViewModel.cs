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
        private DdpFilteredCollection<Post> posts; 

        public DdpFilteredCollection<Post> Posts {
            get { return this.posts; }
            set
            {
                this.posts = value;
                this.OnPropertyChanged();
            } }

        public bool ShowLoadMore
        {
            get { return this.showLoadMore; }
            internal set
            {
                this.showLoadMore = value;
                this.OnPropertyChanged();
            }
        }
    }
}
