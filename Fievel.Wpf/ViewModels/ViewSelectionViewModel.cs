using Fievel.Wpf.Commands;
using Provausio.Common.Portable;

namespace Fievel.Wpf.ViewModels
{
    public class ViewSelectionViewModel : ModelBase
    {
        private string _content;

        public SaveToTextCommand SaveToTextCommand { get; set; }

        public EmailAttachmentCommand EmailAttachmentCommand { get; set; }

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        public ViewSelectionViewModel()
        {
            SaveToTextCommand = new SaveToTextCommand();
            EmailAttachmentCommand = new EmailAttachmentCommand();
        }
    }
}
