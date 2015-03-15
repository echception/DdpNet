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
    public sealed partial class PostPage : Page
    {

        private NavigationHelper navigationHelper;

        private List<Subscription> subscriptions;

        private PostPageViewModel viewModel;

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public PostPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.subscriptions = new List<Subscription>();
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var postId = e.NavigationParameter as String;
            this.subscriptions.Add(await App.Current.Client.Subscribe("singlePost", postId));

            var commentCollection = App.Current.Client.GetCollection<Comment>("comments");

            this.subscriptions.Add(await App.Current.Client.Subscribe("comments", postId));

            var post = App.Current.Client.GetCollection<Post>("posts").Single(x => x.ID == postId);
            var comments = commentCollection.Filter(whereFilter: x => x.PostId == postId);

            this.viewModel = new PostPageViewModel() {Comments = comments, Post = post, Client = App.Current.Client};
            this.DataContext = this.viewModel;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private async void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
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

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

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
