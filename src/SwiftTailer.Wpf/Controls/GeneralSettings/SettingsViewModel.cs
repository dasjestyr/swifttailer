using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using SwiftTailer.Wpf.Infrastructure.Mvvm;

namespace SwiftTailer.Wpf.Controls.GeneralSettings
{
    public class SettingsViewModel : ModelBase
    {
        public int MaxReadLength
        {
            get { return Settings.SeekBuffer; }
            set
            {
                Settings.SeekBuffer = value;
                OnPropertyChanged();
            }
        }

        public int MaxDisplayLines
        {
            get { return Settings.MaxDisplayLogLines; }
            set
            {
                Settings.MaxDisplayLogLines = value;
                OnPropertyChanged();
            }
        }

        public int PollingInterval
        {
            get { return Settings.PollingInterval; }
            set
            {
                Settings.PollingInterval = value;
                OnPropertyChanged();
            }
        }

        public string UserEmail
        {
            get { return Settings.UserEmail; }
            set
            {
                Settings.UserEmail = value;
                OnPropertyChanged();
            }
        }

        public bool AutoFollow
        {
            get { return Settings.AutoFollow; }
            set
            {
                Settings.AutoFollow = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFontFamily
        {
            get { return Settings.LogWindowFont; }
            set
            {
                Settings.LogWindowFont = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> FontNames { get; set; }

        public SettingsViewModel()
        {
            FontNames = new ObservableCollection<string>();
            foreach (var family in Fonts.SystemFontFamilies
                .SelectMany(font => font.FamilyNames)
                .OrderBy(font => font.Value))
            {
                FontNames.Add(family.Value);
            }
        }
    }
}
