using Fievel.Wpf.Commands;
using Fievel.Wpf.Models;

namespace Fievel.Wpf.ViewModels
{
    public class AddGroupViewModel : ModelBase
    {
        private string _groupName;

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                _groupName = value;
                OnPropertyChanged();
            }
        }

        public CreateGroupCommand CreateGroupCommand { get; set; }

        public AddGroupViewModel()
        {
            CreateGroupCommand = new CreateGroupCommand(this);
        }
        
    }
}
