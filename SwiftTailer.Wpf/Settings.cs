using System.Diagnostics;
using System.Windows.Media;
using SwiftTailer.Wpf.Data;

namespace SwiftTailer.Wpf
{
    public class Settings
    {
        /// <summary>
        /// The display buffer in kilobytes (i.e. 1 = 1000 bytes). Default 255. This will control how many kilobytes will be read from the back of the log file. For example, if the file is 1000 bytes and this value is set to 100, then it will only read bytes 900 thru 1000.
        /// </summary>
        public static int SeekBuffer
        {
            get { return SettingsSource.Instance.Settings.MaxReadLength;}
            set
            {
                Debug.WriteLine($"Changed SeekBuffer to {value}");
                SettingsSource.Instance.Settings.MaxReadLength = value;
                SettingsSource.SaveState();
            }
        }

        /// <summary>
        /// The maximum number of lines to display in the log tailing window.
        /// </summary>
        public static int MaxDisplayLogLines
        {
            get { return SettingsSource.Instance.Settings.MaxDisplayLines; }
            set
            {
                Debug.WriteLine($"Changed MaxDisplayLogLines to {value}");
                SettingsSource.Instance.Settings.MaxDisplayLines = value;
                SettingsSource.SaveState();
            }
        }

        /// <summary>
        /// Polling interval in seconds. Default = 1000
        /// </summary>
        public static int PollingInterval
        {
            get { return SettingsSource.Instance.Settings.PollingInterval; }
            set
            {
                Debug.WriteLine($"Changed PollingInterval to {value}");
                SettingsSource.Instance.Settings.PollingInterval = value;
                SettingsSource.SaveState();
            }
        }

        /// <summary>
        /// The font face that will be used in log windows
        /// </summary>
        public static FontFamily LogWindowFontFamily = new FontFamily("Courier New");

        /// <summary>
        /// The directory in which all supporting files should be stored (e.g. logs, user configurations, etc)
        /// </summary>
        public static string WorkingDirectory => SettingsSource.Instance.WorkingDirectory;
    }
}
