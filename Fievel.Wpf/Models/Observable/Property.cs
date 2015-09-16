using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fievel.Wpf.Models.Observable
{
    public class Property<T> : BindableBase, IProperty
    {
        private bool _valueSet;
        private T _value;
        private T _original;

        public T Original
        {
            get { return _original; }
            private set
            {
                _original = value;
                _valueSet = true;
            }
        }

        public T Value
        {
            get { return _value; }
            set
            {
                if (!_valueSet)
                    Original = value;
                SetProperty(ref _value, value);
                OnPropertyChanged("IsDirty");
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public ObservableCollection<string> Errors { get; } = new ObservableCollection<string>();

        public event EventHandler ValueChanged;

        public bool IsEnabled { get; set; }

        public bool IsValid => !Errors.Any();

        public bool IsDirty
        {
            get
            {
                bool dirty;
                if (Value == null && Original == null)
                    dirty = false;
                else if (Value == null && Original != null)
                    dirty = true;
                else
                {
                    dirty = Original.Equals(Value);
                }
                return dirty;
            }
        }

        public Property()
        {
            Errors.CollectionChanged += (s, e) => OnPropertyChanged("IsValid");
        }

        public void Revert()
        {
            Value = Original;
        }

        public void MarkAsClean()
        {
            Original = Value;
            OnPropertyChanged("IsDirty");
        }
    }
}
