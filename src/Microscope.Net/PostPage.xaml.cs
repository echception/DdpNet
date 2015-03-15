using Microscope.Net.Common;
using System;
using System.Collections.Generic;
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
using DdpNet;
using Microscope.Net.DataModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Microscope.Net
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PostPage : BasePage
    {
        private List<Subscription> subscriptions;

        private PostPageViewModel viewModel;

        public PostPage()
        {
            this.InitializeComponent();
            this.subscriptions = new List<Subscription>();
        }

        protected override async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var postId = e.NavigationParameter as String;
            this.subscriptions.Add(await App.Current.Client.Subscribe("singlePost", postId));

            var commentCollection = App.Current.Client.GetCollection<Comment>("comments");

            this.subscriptions.Add(await App.Current.Client.Subscribe("comments", postId));

            var post = App.Current.Client.GetCollection<Post>("posts").Single(x => x.ID == postId);
            var comments = commentCollection.Filter(whereFilter: x => x.PostId == postId);

            this.viewModel = new PostPageViewModel() { Comments = comments, Post = post };
            this.DataContext = this.viewModel;
        }

        protected override async void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (this.subscriptions != null)
            {
                foreach (var subscription in this.subscriptions)
                {
                    await App.Current.Client.Unsubscribe(subscription);
                }
                this.subscriptions.Clear();
            }
        }

        private async void AddCommentButton_OnClick(object sender, RoutedEventArgs e)
        {
            var text = this.CommentTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                var comment = new CommentSubmit(text, this.viewModel.Post.ID);

                await App.Current.Client.Call("commentInsert", comment);
                this.CommentTextBox.Text = string.Empty;
            }
        }
    }
}
