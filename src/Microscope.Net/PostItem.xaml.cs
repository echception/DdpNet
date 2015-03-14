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
using Microscope.Net.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Microscope.Net
{
    public sealed partial class PostItem : UserControl
    {
        public PostItem()
        {
            this.InitializeComponent();
        }

        private void UpvoteButton_OnClick(object sender, RoutedEventArgs e)
        {
            Post post = (Post)((Button)e.OriginalSource).DataContext;
            App.Current.Client.Call("upvote", post.ID);
        }
    }
}
