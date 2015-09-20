using SwiftTailer.Wpf.Commands;

namespace SwiftTailer.Wpf.ViewModels
{
    public class ViewSelectionViewModel : Models.Observable.ModelBase
    {
        private string _content;

        public SaveToTextCommand SaveToTextCommand { get; set; }

        public EmailSelectionAttachmentCommand EmailSelectionAttachmentCommand { get; set; }

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
            EmailSelectionAttachmentCommand = new EmailSelectionAttachmentCommand();
        }
    }
}
