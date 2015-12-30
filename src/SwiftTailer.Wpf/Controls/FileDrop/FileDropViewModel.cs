using System;
using SwiftTailer.Wpf.Infrastructure;
using SwiftTailer.Wpf.Infrastructure.Mvvm;
using SwiftTailer.Wpf.Models;

namespace SwiftTailer.Wpf.Controls.FileDrop
{
    public class FileDropViewModel : ModelBase
    {
        private string _message;
        private string _groupName;

        public FileDropResultArgs CancelArgs { get; set; }

        public FileDropResultArgs AddArgs { get; set; }

        public FileDropResultArgs NewGroupArgs { get; set; }

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                _groupName = value;
                NewGroupArgs.GroupName = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public FileDropViewModel(MainViewModel hostVm, FileInfoCollection fileInfos)
        {
            Message = $"Would you like to create a new group or add to the active group ({hostVm.SelectedGroup.Name})?";
            CancelArgs = new FileDropResultArgs(string.Empty, fileInfos, FileDropOption.None);
            AddArgs = new FileDropResultArgs(hostVm.SelectedGroup.Name, fileInfos, FileDropOption.Add);
            NewGroupArgs = new FileDropResultArgs(GroupName, fileInfos, FileDropOption.New);
        }
    }

    public class FileDropResultArgs : EventArgs
    {
        public string GroupName { get; set; }

        public FileInfoCollection FileInfos { get; set; }

        public FileDropOption Option { get; set; }

        public FileDropResultArgs(string groupName, FileInfoCollection fileInfos, FileDropOption option)
        {
            GroupName = groupName;
            FileInfos = fileInfos;
            Option = option;
        }
    }

    public enum FileDropOption
    {
        None,
        New,
        Add
    }
}
