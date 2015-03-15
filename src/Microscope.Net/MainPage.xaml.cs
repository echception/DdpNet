using Microscope.Net.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Microscope.Net
{
    using System.Threading.Tasks;
    using DataModel;
    using DdpNet;

    public sealed partial class MainPage : BasePage
    {
        private MainPageViewModel viewModel;

        private int limit;
        private Sort sort;
        private const int Increment = 5;

        private Subscription currentSubscription;


        public MainPage() : base()
        {
            this.InitializeComponent();
        }

        private async void Load()
        {
            var collection = App.Current.Client.GetCollection<Post>("posts");
            this.limit = Increment;
            this.sort = new Sort() {ID = -1, Submitted = -1};

            this.viewModel = new MainPageViewModel(App.Current.Client, collection, false);

            ((INotifyCollectionChanged)collection).CollectionChanged +=
    (sender, args) => this.viewModel.ShowLoadMore = this.viewModel.Posts.Count >= this.limit;

            this.DataContext = this.viewModel;

            await this.LoadData();
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

        protected override void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.Load();
        }

        protected override void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private async void LoadMoreButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.viewModel.ShowLoadMore)
            {
                this.limit += Increment;
                await this.LoadData();
            }
        }

        private void Discuss_OnClick(object sender, RoutedEventArgs e)
        {
            var post = (Post) ((Button) e.OriginalSource).DataContext;
            this.Frame.Navigate(typeof (PostPage), post.ID);
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            var post = (Post) ((Button) e.OriginalSource).DataContext;
            this.Frame.Navigate(typeof (EditPostPage), post.ID);
        }

    }
}
