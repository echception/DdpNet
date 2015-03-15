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
using DdpNet.Packages.Accounts;
using Microscope.Net.DataModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Microscope.Net
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class LoginPage : BasePage
    {

        private LoginPageViewModel viewModel;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        protected override void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.viewModel = new LoginPageViewModel();
            this.DataContext = this.viewModel;
        }

        protected override void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void CreateAccount_OnClick(object sender, RoutedEventArgs e)
        {
            this.viewModel.CreateUser = true;
            this.viewModel.ErrorText = string.Empty;
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs args)
        {
            var client = App.Current.Client;
            if (string.IsNullOrWhiteSpace(this.viewModel.UserName))
            {
                this.viewModel.ErrorText = "UserName required";
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.viewModel.Password))
            {
                this.viewModel.ErrorText = "Password required";
                return;
            }

            try
            {
                await client.LoginPassword(this.viewModel.UserName, this.viewModel.Password);
                NavigationHelper.GoBack();
            }
            catch (DdpServerException e)
            {

                this.viewModel.ErrorText = e.Reason;
            }
        }

        private async void CreateUserButton_OnClick(object sender, RoutedEventArgs args)
        {
            var client = App.Current.Client;
            if (string.IsNullOrWhiteSpace(this.viewModel.UserName))
            {
                this.viewModel.ErrorText = "UserName required";
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.viewModel.Password))
            {
                this.viewModel.ErrorText = "Password required";
                return;
            }

            if (string.IsNullOrWhiteSpace(this.viewModel.PasswordAgain))
            {
                this.viewModel.ErrorText = "Enter password again";
            }
            else if (this.viewModel.Password != this.viewModel.PasswordAgain)
            {
                this.viewModel.ErrorText = "Passwords do not match";
            }
            else
            {
                try
                {
                    await client.CreateUserWithUserName(this.viewModel.UserName, this.viewModel.Password);
                    NavigationHelper.GoBack();
                }
                catch (DdpServerException e)
                {
                    this.viewModel.ErrorText = e.Reason;
                }
            }
        }
    }
}
