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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microscope.Net
{
    using Common;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BestPage : BasePage
    {
        public BestPage()
        {
            this.InitializeComponent();
        }

        protected override void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.PostList.Initialize(new BestSort() { Votes = -1, ID = -1, Submitted = -1 }, (post1, post2) =>
            {
                var votesComparison = post2.Votes.CompareTo(post1.Votes);

                if (votesComparison != 0)
                {
                    return votesComparison;
                }

                var timeComparison = post2.Submitted.DateTime.CompareTo(post1.Submitted.DateTime);

                if (timeComparison != 0)
                {
                    return timeComparison;
                }

                return post2.ID.CompareTo(post1.ID);
            });
        }

        protected override void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
 
        }
    }
}
