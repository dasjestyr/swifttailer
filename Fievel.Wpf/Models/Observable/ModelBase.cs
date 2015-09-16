using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Fievel.Wpf.Models.Observable
{
    public abstract class ModelBase : BindableBase, IModel
    {
        public ObservableCollection<string> Errors { get; } = new ObservableCollection<string>();

        public bool IsDirty
        {
            get { return Properties.Values.Any(p => p.IsDirty); }
        }

        public bool IsValid { get { return Properties.Values.All(p => p.IsValid) && !Errors.Any(); } }

        public Action<IModel> Validator { get; set; }

        public Dictionary<string, IProperty> Properties { get; } = new Dictionary<string, IProperty>();

        protected ModelBase()
        {
            foreach (var property in GetType().GetRuntimeProperties())
            {
                var type = typeof(Property<>).MakeGenericType(property.PropertyType);
                var prop = (IProperty)Activator.CreateInstance(type);
                Properties.Add(property.Name, prop);

                prop.ValueChanged += BindEventBubbling;
            }
        }

        protected virtual void BindEventBubbling(object sender, EventArgs e) { }

        public void SetProperty<TPropertyType>(TPropertyType value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) return;
            var property = Properties[propertyName] as Property<TPropertyType>;

            if (property != null)
                property.Value = value;
        }

        public void Revert()
        {
            foreach (var property in Properties.Values)
            {
                property.Revert();
            }
        }

        public void Validate()
        {
            // reset errors
            Errors.Clear();

            var properties = Properties.Values;
            foreach (var property in properties)
            {
                property.Errors.Clear();
            }

            // validate
            Validator?.Invoke(this);

            foreach (var error in properties.SelectMany(p => p.Errors))
            {
                Errors.Add(error);
            }
        }

        public void MarkAsClean()
        {
            foreach (var property in Properties)
            {
                property.Value.MarkAsClean();
                OnPropertyChanged("IsDirty");
            }
        }
    }
}