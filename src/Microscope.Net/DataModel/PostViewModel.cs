using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Microscope.Net.DataModel
{
    public class PostViewModel : INotifyPropertyChanged
    {
        private string error;
        private string title;
        private string url;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Error
        {
            get { return this.error; }
            set
            {
                this.error = value;
                this.OnPropertyChanged();
            }
        }

        public string Url
        {
            get { return this.url; }
            set
            {
                this.url = value;
                this.OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return this.title; }
            set
            {
                this.title = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
