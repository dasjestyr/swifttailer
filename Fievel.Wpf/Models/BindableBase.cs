using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fievel.Wpf.Models
{
    public class BindableBase : INotifyPropertyChanged
    {
        public void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            // if the values are the same, ignore it
            if (Equals(storage, value)) return false;

            // else update the property
            storage = value;

            // and notify subscribers
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}