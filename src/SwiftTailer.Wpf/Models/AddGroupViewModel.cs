using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Data;

namespace SwiftTailer.Wpf.Models
{
    public class AddGroupViewModel : ModelBase
    {
        private LogGroup _group;

        public string GroupName
        {
            get { return Group.Name; }
            set
            {
                Group.Name = value;
                OnPropertyChanged();
            }
        }

        public LogGroup Group
        {
            get { return _group; }
            set
            {
                _group = value;
                OnPropertyChanged();
                OnPropertyChanged("GroupName");
            }
        }

        public CreateGroupCommand CreateGroupCommand { get; set; }

        public EditGroupCommand EditGroupCommand { get; set; }

        public AddGroupViewModel()
        {
            Group = new LogGroup();
            CreateGroupCommand = new CreateGroupCommand(this);
            EditGroupCommand = new EditGroupCommand(this);
        }
        
    }
}
