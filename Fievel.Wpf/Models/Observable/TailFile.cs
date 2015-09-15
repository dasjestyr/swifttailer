using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using Fievel.Wpf.Commands;
using Fievel.Wpf.Data;
using System.Windows;

namespace Fievel.Wpf.Models.Observable
{
    public class TailFile : ModelBase
    {
        private readonly object _lockObject = new object();
        private readonly long _displayBuffer;
        private CancellationTokenSource _cts;
        private string _logText;
        private long _lastIndex;
        private ObservableCollection<LogLine> _logLines;
        private string _searchPhrase;

        public event RawContentsChangedHandler RawContentChanged;

        /// <summary>
        /// Gets or sets the log name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name => LogInfo.Alias;

        /// <summary>
        /// Gets or sets the log identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id => LogInfo.Id;

        /// <summary>
        /// Gets or sets the text changed command. Used to apply highlighting.
        /// </summary>
        /// <value>
        /// The text changed command.
        /// </value>
        public ApplyHighlightingCommand ApplyHighlightingCommand { get; set; }

        public OpenInExplorerCommand OpenInExplorerCommand { get; set; }

        public RemoveLogFromGroupCommand RemoveLogFromGroupCommand { get; set; }

        #region -- Observable Properties --

        /// <summary>
        /// Gets or sets the raw contents.
        /// </summary>
        /// <value>
        /// The raw contents.
        /// </value>
        public string LogText
        {
            get { return _logText; }
            set
            {
                var originalText = _logText;
                _logText = value;
                OnPropertyChanged();
                OnLogTextChanged(new RawContentsChangedEventArgs(originalText, _logText, Id));
            }
        }

        /// <summary>
        /// Gets or sets the log lines.
        /// </summary>
        /// <value>
        /// The log lines.
        /// </value>
        public ObservableCollection<LogLine> LogLines
        {
            get { return _logLines; }
            set
            {
                _logLines = value;
                OnPropertyChanged();
            }
        }

        public string SearchPhrase
        {
            get { return _searchPhrase; }
            set
            {
                _searchPhrase = value;
                OnPropertyChanged();
            }
        }

        public int Order
        {
            get { return LogInfo.Order; }
            set
            {
                LogInfo.Order = value;
                OnPropertyChanged();
            }
        }

        #endregion
        
        /// <summary>
        /// Gets the log information.
        /// </summary>
        /// <value>
        /// The log information.
        /// </value>
        public LogInfo LogInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TailFile" /> class.
        /// </summary>
        /// <param name="logInfo">The log information.</param>
        public TailFile(LogInfo logInfo)
        {
            LogInfo = logInfo;
            _displayBuffer = Settings.DisplayBufferSize * 0x400;
            _cts = new CancellationTokenSource();

            ApplyHighlightingCommand = new ApplyHighlightingCommand(this);
            RemoveLogFromGroupCommand = new RemoveLogFromGroupCommand(this);
            OpenInExplorerCommand = new OpenInExplorerCommand();
            LogLines = new ObservableCollection<LogLine>();

            BindingOperations.EnableCollectionSynchronization(LogLines, _lockObject);
        }

        public void DeleteSelf()
        {
            // make sure this isn't running
            _cts.Cancel();

            // this tail has a unique id generated on startup so
            // just ask the singleton to blow it away
            LogSource.Instance.Logs.Groups.First(
                log => log.Logs
                .Any(l => l.Id.Equals(LogInfo.Id, StringComparison.OrdinalIgnoreCase)))
                .Logs.Remove(LogInfo);
                        
            LogSource.Instance.SaveState();
            Trace.WriteLine($"Deleted {LogInfo.Alias} from configuration.");
        }

        /// <summary>
        /// Starts the tailing.
        /// </summary>
        /// <returns></returns>
        public async Task StartTailing()
        {
            _cts = new CancellationTokenSource();
            
            if (File.Exists(LogInfo.Location))
            {
                Trace.WriteLine($"Starting tail on {LogInfo.Alias}...");
                await Task.Run(async () =>
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        await GetUpdates();
                        await Task.Delay(Settings.PollingInterval);
                    }
                }, _cts.Token);
            }
            else
            {
                await Task.Run(() =>
                {
                    Trace.WriteLine($"{LogInfo.Location} was not found.");
                    LogText = $"File not found: {LogInfo.Location}";
                });
            }
        }

        /// <summary>
        /// Stops the tailing.
        /// </summary>
        public void StopTailing()
        {
            Trace.WriteLine($"Called tail cancellation on {LogInfo.Alias}");
            _cts.Cancel();
        }
        
        private async Task GetUpdates()
        {
            var originalText = LogText;
            long messageSize = 0;
            long newContentSize = 0;

            try
            {
                lock (_lockObject)
                {
                    using (var fs = new FileStream(LogInfo.Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (fs.Length == 0 || !fs.CanRead || fs.Length == _lastIndex)
                            return; // no change

                        // avoid reading the entire file on startup
                        var startAt = _lastIndex;
                        if (startAt == 0 && fs.Length > _displayBuffer)
                        {
                            startAt = fs.Length - _displayBuffer;
                            Debug.WriteLine(
                                $"{LogInfo.Alias} file was larger than buffer ({_displayBuffer}). Starting at i={startAt} instead of beginning.");
                        }

                        newContentSize = fs.Length - startAt;
                        var newContent = new byte[newContentSize];
                        Debug.WriteLine($"This chunk will be {newContent.Length} bytes.");

                        fs.Seek(startAt, SeekOrigin.Begin);

                        // read the new data
                        messageSize = fs.Read(newContent, 0, newContent.Length);

                        // trim off the front of the content if it exceeds the display buffer
                        var trimmedContent = originalText + Encoding.UTF8.GetString(newContent);
                        if (!string.IsNullOrEmpty(LogText) && trimmedContent.Length > _displayBuffer)
                        {
                            trimmedContent = new string(trimmedContent.Skip((int)(trimmedContent.Length - _displayBuffer)).ToArray());
                        }

                        // update the model
                        LogText = trimmedContent;

                        OnLogTextChanged(new RawContentsChangedEventArgs(originalText, LogText, Id));

                        Debug.WriteLine($"{LogInfo.Alias} content is now {LogText.Length} characters.");
                        _lastIndex = startAt;
                        _lastIndex = fs.Position;
                    }
                }
                
            }
            catch (IOException ex)
            {
                
                MessageBox.Show($"Message size: {messageSize} New Content Size: {newContentSize} :: " + ex.Message, "Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnLogTextChanged(RawContentsChangedEventArgs args)
        {
            Debug.WriteLine("RawContentChanged event fired");

            LogLines.Clear();
            LogLines.AddRange(args.NewText
                .Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => new LogLine(line, LogHighlight.None)));

            // trigger listeners
            RawContentChanged?.Invoke(this, args);
        }
    }
}
