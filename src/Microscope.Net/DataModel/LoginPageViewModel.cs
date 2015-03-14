using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Microscope.Net.DataModel
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool createUser;
        private string errorText;
        private string userName;
        private string password;
        private string passwordAgain;

        public bool CreateUser
        {
            get { return this.createUser; }
            set
            {
                this.createUser = value;
                this.OnPropertyChanged();
            }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; this.OnPropertyChanged(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value; this.OnPropertyChanged();}
        }

        public string PasswordAgain
        {
            get { return passwordAgain; }
            set { passwordAgain = value; this.OnPropertyChanged();}
        }

        public string ErrorText
        {
            get { return this.errorText; }
            set
            {
                this.errorText = value;
                this.OnPropertyChanged();
            }
        }

        public LoginPageViewModel()
        {
            this.createUser = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
