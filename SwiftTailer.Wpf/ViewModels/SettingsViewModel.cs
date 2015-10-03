using SwiftTailer.Wpf.Models.Observable;

namespace SwiftTailer.Wpf.ViewModels
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
    }
}
