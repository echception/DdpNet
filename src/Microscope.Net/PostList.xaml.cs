namespace Microscope.Net
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using DataModel;
    using DdpNet;

    public sealed partial class PostList : UserControl
    {
        private PostListViewModel viewModel;
        private int limit;
        private object sort;
        private const int Increment = 5;
        private Subscription currentSubscription;

        public PostList()
        {
            this.InitializeComponent();
        }

        public async Task Initialize(PostListViewModel viewModel, object sort, Comparison<Post> sortMethod )
        {
            var collection = App.Current.Client.GetCollection<Post>("posts");
            this.limit = Increment;
            this.sort = sort;

            this.viewModel = viewModel;
            this.viewModel.ShowLoadMore = false;
            this.viewModel.Posts = collection.Filter(sortFilter: sortMethod);

            ((INotifyCollectionChanged)collection).CollectionChanged +=
    (sender, args) => this.viewModel.ShowLoadMore = this.viewModel.Posts.Count >= this.limit;

            await this.LoadData();
        }

        private async void LoadMoreButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.viewModel.ShowLoadMore)
            {
                this.limit += Increment;
                await this.LoadData();
            }
        }

        private async Task LoadData()
        {
            var newSubscription = await App.Current.Client.Subscribe("posts", new SubscribeParamters { Limit = this.limit, Sort = this.sort });

            if (this.currentSubscription != null)
            {
                await App.Current.Client.Unsubscribe(this.currentSubscription);
            }

            this.currentSubscription = newSubscription;
            this.viewModel.ShowLoadMore = this.viewModel.Posts.Count >= this.limit;
        }

        private void Discuss_OnClick(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)Window.Current.Content;
            var post = (Post)((Button)e.OriginalSource).DataContext;

            frame.Navigate(typeof(PostPage), post.Id);
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame frame = (Frame)Window.Current.Content;

            var post = (Post)((Button)e.OriginalSource).DataContext;
            frame.Navigate(typeof(EditPostPage), post.Id);
        }
    }
}
