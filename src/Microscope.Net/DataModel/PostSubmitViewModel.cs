using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Microscope.Net.DataModel
{
    public class PostSubmitViewModel : INotifyPropertyChanged
    {
        private string error;
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
