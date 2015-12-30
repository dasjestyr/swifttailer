using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Infrastructure.Mvvm;

namespace SwiftTailer.Wpf.Models
{
    public class ViewSelectionViewModel : ModelBase
    {
        private string _content;
        private IEnumerable<LogLine> _logLines;
        private string _logFont = Settings.LogWindowFont;

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

        public string LogFont
        {
            get { return _logFont; }
            set
            {
                _logFont = value;
                OnPropertyChanged();
            }
        }

        public ViewSelectionViewModel()
        {
            SaveToTextCommand = new SaveToTextCommand();
            EmailSelectionAttachmentCommand = new EmailSelectionAttachmentCommand();
        }

        public void SettingsChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals("LogWindowFont", StringComparison.OrdinalIgnoreCase))
            {
                LogFont = Settings.LogWindowFont;
            }
        }
    }
}
