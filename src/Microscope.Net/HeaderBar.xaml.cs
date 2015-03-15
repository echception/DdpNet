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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Microscope.Net
{
    public sealed partial class HeaderBar : UserControl
    {
        public static readonly DependencyProperty SelectedMenuIndexProperty =
            DependencyProperty.Register("SelectedMenuIndex", typeof (int), typeof (HeaderBar), new PropertyMetadata(-1, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            HeaderBar control = (HeaderBar) dependencyObject;

            control.NavigationTabs.SelectedIndex = (int)dependencyPropertyChangedEventArgs.NewValue;
        }

        public int SelectedMenuIndex
        {
            get { return (int) GetValue(SelectedMenuIndexProperty); }
            set { SetValue(SelectedMenuIndexProperty, value); }
        }

        public HeaderBar()
        {
            this.InitializeComponent();
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof (LoginPage));
        }

        private void NavigationTabs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = this.NavigationTabs.SelectedIndex;

            if (index == this.SelectedMenuIndex)
            {
                return;
            }

            var frame = (Frame) Window.Current.Content;

            switch (index)
            {
                case 0:
                    frame.Navigate(typeof (MainPage));
                    break;
                case 1:
                    frame.Navigate(typeof (BestPage));
                    break;
                case 2:
                    frame.Navigate(typeof (SubmitPost));
                    break;
            }

            this.NavigationTabs.SelectedIndex = this.SelectedMenuIndex;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);

            while (!(parent is Page))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent is BasePage)
            {
                var page = parent as BasePage;
                if (page.NavigationHelper.CanGoBack())
                {
                    page.NavigationHelper.GoBack();
                }
            }
        }
    }
}
