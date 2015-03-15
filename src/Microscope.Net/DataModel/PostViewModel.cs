using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Microscope.Net.DataModel
{
    public class PostViewModel : BaseViewModel
    {
        private string error;
        private string title;
        private string url;

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
    }
}
