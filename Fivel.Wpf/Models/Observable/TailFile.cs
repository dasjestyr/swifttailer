using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fivel.Wpf.Data;
using Provausio.Common.Portable;

namespace Fivel.Wpf.Models.Observable
{
    public class TailFile : ModelBase
    {
        private readonly long _displayBuffer;
        private readonly TimeSpan _interval;
        private readonly CancellationTokenSource _cts;
        private string _contents;
        private long _lastIndex;


        public string Contents
        {
            get { return _contents; }
            set
            {
                _contents = value;
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

        public string Name => LogInfo.Alias;

        public string Id => LogInfo.Id;

        public LogInfo LogInfo { get; }

        public TailFile(LogInfo logInfo)
        {
            LogInfo = logInfo;
            _displayBuffer = Settings.DisplayBufferSize * 1000;
            _interval = new TimeSpan(Settings.PollingInterval);
            _cts = new CancellationTokenSource();
        }

        public Task StartTailing()
        {
            Trace.WriteLine("Starting...");
            return DoTail();
        }

        public void StopTailing()
        {
            Trace.WriteLine("Called cancellation");
            _cts.Cancel();
        }

        private async Task DoTail()
        {
            await Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    await UpdateFile();
                    await Task.Delay(_interval);
                }
            }, _cts.Token);

            Debug.WriteLine($"Stopped tailing {LogInfo.Alias}");
        }

        private async Task UpdateFile()
        {
            try
            {
                if (!File.Exists(LogInfo.Location))
                {
                    Trace.WriteLine($"{LogInfo.Location} was not found.");
                    Contents = $"File not found: {LogInfo.Location}";
                    _cts.Cancel();
                    return;
                }

                using (var fs = new FileStream(LogInfo.Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (!fs.CanRead || fs.Length == _lastIndex) return; // no change

                    // avoid reading the entire file on startup
                    long startAt = _lastIndex;
                    if (startAt == 0 && fs.Length > _displayBuffer)
                    {
                        startAt = fs.Length - _displayBuffer;
                        Trace.WriteLine($"{LogInfo.Alias} file was larger than buffer ({_displayBuffer}). Starting at i={startAt} instead of beginning.");
                    }

                    long contentLength = fs.Length - startAt;
                    if (contentLength < 0)
                    {
                        // trying to track some weird overflow case
                        Trace.WriteLine($"!!!!!! Length: {fs.Length} startAt: {startAt}");
                        return;
                    }

                    var newContent = new byte[contentLength];
                    Trace.WriteLine($"This chunk will be {newContent.Length} bytes.");

                    fs.Seek(startAt, SeekOrigin.Begin);

                    var bytesRead = await fs.ReadAsync(newContent, 0, newContent.Length);

                    // trim the buffer off the front
                    var trimmedContent = Contents;
                    if (!string.IsNullOrEmpty(Contents) && Contents.Length + bytesRead > _displayBuffer)
                    {
                        Trace.WriteLine("Windows contents exceed the display buffer size. Trimming off the top...");
                        var skipLength = (_displayBuffer - bytesRead) < Contents.Length ? 0 : _displayBuffer - bytesRead;
                        var contentBytes = Encoding.UTF8.GetBytes(Contents).Skip((int)skipLength).ToArray();
                        trimmedContent = Encoding.UTF8.GetString(contentBytes);
                    }

                    // update the model
                    Contents = trimmedContent + Encoding.UTF8.GetString(newContent);
                    Trace.WriteLine($"Content is now {Contents.Length} characters.");
                    _lastIndex = startAt;
                    _lastIndex = fs.Position;
                }
            }
            catch (IOException) { }
        }
    }
}
