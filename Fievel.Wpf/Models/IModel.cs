using System;
using System.Collections.ObjectModel;

namespace Fievel.Wpf.Models
{
    public interface IModel
    {
        ObservableCollection<string> Errors { get; }

        bool IsDirty { get; }

        bool IsValid { get; }

        Action<IModel> Validator { get; }

        void Revert();

        void Validate();

        void MarkAsClean();
    }
}