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
        public MainPage() : base()
        {
            this.InitializeComponent();
        }

        protected override void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.PostList.Initialize(new Sort() { ID = -1, Submitted = -1 }, (post1, post2) => post2.Submitted.DateTime.CompareTo(post1.Submitted.DateTime));
        }

        protected override void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }
    }
}
