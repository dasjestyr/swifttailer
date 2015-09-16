using Fievel.Wpf.Commands;
using ModelBase = Fievel.Wpf.Models.Observable.ModelBase;

namespace Fievel.Wpf.ViewModels
{
    public class ViewSelectionViewModel : Models.Observable.ModelBase
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
