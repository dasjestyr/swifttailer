using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.Filters;

namespace SwiftTailer.Wpf.Models.Observable
{
    public class TailFile : ModelBase
    {
        private readonly object _lockObject = new object();
        private readonly long _displayBuffer;
        private CancellationTokenSource _cts;
        private string _logText;
        private long _lastIndex;
        private ObservableCollection<LogLine> _logLines;
        private bool _lastLineIsDirty;
        private string _searchPhrase;
        private int _lineCount;
        private int _selectedLineIndex;
        private bool _followTail;

        public event RawContentsChangedHandler RawContentChanged;
        public event NewContentAddedHandler NewLinesAdded;

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
        public ApplyUserInputHighlightCommand ApplyUserInputHighlightCommand { get; set; }

        public OpenInExplorerCommand OpenInExplorerCommand { get; set; }

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

        public int LineCount
        {
            get { return _lineCount; }
            set
            {
                _lineCount = value;
                OnPropertyChanged();
            }
        }

        public int SelectedLine
        {
            get { return _selectedLineIndex; }
            set
            {
                _selectedLineIndex = value;
                OnPropertyChanged();
            }
        }

        public bool FollowTail
        {
            get { return _followTail; }
            set
            {
                _followTail = value;
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

            ApplyUserInputHighlightCommand = new ApplyUserInputHighlightCommand(this);
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
            LogSource.Instance.RemoveLog(LogInfo);
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
                await RunUpdates();            
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

        private async Task RunUpdates()
        {
            Trace.WriteLine($"Starting tail on {LogInfo.Alias}...");
            await Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    GetUpdates();

                    if (FollowTail && _lastLineIsDirty)
                    {
                        SelectedLine = LogLines.Count - 1;
                        _lastLineIsDirty = false;
                    }

                    await Task.Delay(Settings.PollingInterval);
                }
            }, _cts.Token);
        }
        
        private void GetUpdates()
        {
            long messageSize = 0;
            long newContentSize = 0;
            
            // lock this or else you'll run into internal array sizing issues with the bounded ItemSource
            lock (_lockObject)
            {
                // handle case where file may have been deleted while tailing
                if (!File.Exists(LogInfo.Location))
                {
                    // we want to keep tailing for now in case it was just a case of the file be republished, for example
                    // so just skip
                    Trace.WriteLine("File disappeared. Ignoring until next pass.");
                    return;
                }                    

                using (var fs = new FileStream(LogInfo.Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // handle case where the file may have been reset (e.g. republished)
                    if (_lastIndex > fs.Length)
                    {
                        Trace.WriteLine("File shrank. Assuming it's new and reseting lastIndex to zero!");
                        _lastIndex = 0;
                    }

                    // fs.Length == 0 is just handling a case where filestream returns 0 for no apparent reason
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

                    // create a container for the new data
                    newContentSize = fs.Length - startAt;
                    var newContent = new byte[newContentSize];
                    Debug.WriteLine($"This chunk will be {newContent.Length} bytes.");

                    // fast forward to our starting point
                    fs.Seek(startAt, SeekOrigin.Begin);

                    // read the new data
                    messageSize = fs.Read(newContent, 0, newContent.Length);
                        
                    // detect new lines before attempting to update
                    // if there aren't any, treat it as if the file is untouched
                    var newContentString = Encoding.UTF8.GetString(newContent);
                    if (LogLines.Count > 1 && newContentString.IndexOf(Environment.NewLine) == -1)
                        return;

                    // get the new lines
                    var newLines = newContentString
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => new LogLine(line, LogHighlight.None))
                        .ToList();


                    // update the log collection
                    LogLines.AddRange(newLines);
                    _lastLineIsDirty = true;

                    // trim the log if necessary
                    TrimLog(newLines);                        
                        
                    _lastIndex = startAt;
                    _lastIndex = fs.Position;
                }
            }                       
        }

        private void TrimLog(List<LogLine> newLines)
        {
            if ((LogLines.Count + newLines.Count) >= Settings.MaxDisplayLogLines
                            && LogLines.Count > newLines.Count)
            {
                for (var i = 0; i <= LogLines.Count || i < newLines.Count; i++)
                {
                    LogLines.RemoveAt(i);
                }
            }
            LineCount = LogLines.Count;
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

        private void OnNewContentedAdded(NewContentEventArgs args)
        {
            Debug.WriteLine($"OnNewContentAdded fired (TailFile: {LogInfo.Alias})");
            NewLinesAdded?.Invoke(this, args);
        }
    }

    public delegate void RawContentsChangedHandler(object sender, RawContentsChangedEventArgs args);
    public delegate void NewContentAddedHandler(object sender, NewContentEventArgs args);

    public class NewContentEventArgs : EventArgs
    {
        public TailFile Context { get; private set; }

        public IEnumerable<string> NewLines { get; private set; }

        public int NewLineCount { get; private set; }

        public NewContentEventArgs(TailFile context, IEnumerable<string> newLines)
        {
            Context = context;
            NewLines = newLines;
            NewLineCount = newLines.Count();
        }
    }
}
