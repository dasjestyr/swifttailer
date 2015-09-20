using System.Collections.Generic;
using System.Linq;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.ViewModels
{
    public class ViewSelectionViewModel : Models.Observable.ModelBase
    {
        private string _content;
        private IEnumerable<LogLine> _logLines;

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

        public IEnumerable<LogLine> LogLines
        {
            get { return _logLines; }
            set
            {
                _logLines = value;
                Content = string.Join("\n", value.Select(line => line.Content));
            }
        }

        public ViewSelectionViewModel()
        {
            SaveToTextCommand = new SaveToTextCommand();
            EmailSelectionAttachmentCommand = new EmailSelectionAttachmentCommand();
        }
    }
}
