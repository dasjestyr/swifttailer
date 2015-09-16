namespace Fievel.Wpf
{
    public class Settings
    {
        /// <summary>
        /// The display buffer in kilobytes (i.e. 1 = 1000 bytes). Default 255. This will control how many kilobytes will be displayed in the log window.
        /// </summary>
        public static int DisplayBufferSize { get; set; } = 255;
        
        /// <summary>
        /// Polling interval in seconds. Default = 1000
        /// </summary>
        public static int PollingInterval { get; set; } = 1000; // Do NOT change this to use a timespan!
    }
}
