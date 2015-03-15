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

namespace Microscope.Net
{
    public sealed partial class EditPostPage : BasePage
    {
        private PostViewModel viewModel;
        private Subscription subscription;
        private Post postToEdit;

        public EditPostPage()
        {
            this.InitializeComponent();
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
        protected override async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
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
        protected override async void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (this.subscription != null)
            {
                await App.Current.Client.Unsubscribe(this.subscription);
                this.subscription = null;
            }
        }

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
