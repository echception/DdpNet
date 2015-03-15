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
    public sealed partial class EditPostPage : Page
    {

        private NavigationHelper navigationHelper;
        private PostViewModel viewModel;
        private Subscription subscription;
        private Post postToEdit;

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public EditPostPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
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
            this.subscription = await App.Current.Client.Subscribe("singlePost", postId);

            var post = App.Current.Client.GetCollection<Post>("posts").Single(x => x.ID == postId);

            this.viewModel = new PostViewModel() {Title = post.Title, Url = post.Url};
            this.DataContext = this.viewModel;
            this.postToEdit = post;
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
            if (this.subscription != null)
            {
                await App.Current.Client.Unsubscribe(this.subscription);
                this.subscription = null;
            }
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
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

        private async void SubmitPostButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.viewModel.Url))
            {
                this.viewModel.Error = "Url Required";
                return;
            }

            if (string.IsNullOrWhiteSpace(this.viewModel.Title))
            {
                this.viewModel.Error = "Title Required";
                return;
            }

            PostEdit post = new PostEdit(this.viewModel.Url, this.viewModel.Title);

            var posts = App.Current.Client.GetCollection<Post>("posts");
            await posts.UpdateAsync(this.postToEdit.ID, post);

            this.Frame.Navigate(typeof (PostPage), this.postToEdit.ID);
        }

        private async void DeletePostButton_OnClick(object sender, RoutedEventArgs e)
        {
            var posts = App.Current.Client.GetCollection<Post>("posts");
            await posts.RemoveAsync(this.postToEdit.ID);

            this.Frame.Navigate(typeof (MainPage));
        }
    }
}
