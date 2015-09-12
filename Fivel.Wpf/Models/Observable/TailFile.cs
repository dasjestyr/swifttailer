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
        private readonly LogInfo _logInfo;
        private readonly int _displayBuffer;
        private readonly TimeSpan _interval;
        private readonly CancellationTokenSource _cts;
        private string _contents;
        private int _lastIndex;

        public string Contents
        {
            get { return _contents; }
            set
            {
                _contents = value;
                Trace.WriteLine(value);
                OnPropertyChanged();
            }
        }

        public string Name => _logInfo.Alias;

        public TailFile(LogInfo logInfo)
        {
            _logInfo = logInfo;
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

            Debug.WriteLine($"Stopped tailing {_logInfo.Alias}");
        }

        private async Task UpdateFile()
        {
            try
            {
                if (!File.Exists(_logInfo.Location))
                {
                    Contents = $"File not found: {_logInfo.Location}";
                    _cts.Cancel();
                    return;
                }

                using (var fs = new FileStream(_logInfo.Location, FileMode.Open))
                {
                    if (!fs.CanRead || fs.Length == _lastIndex) return; // no change

                    var newContent = new byte[fs.Length - _lastIndex];
                    fs.Seek(_lastIndex, SeekOrigin.Begin);

                    var bytesRead = await fs.ReadAsync(newContent, 0, newContent.Length);

                    // trim the buffer off the front
                    var trimmedContent = Contents;
                    if (!string.IsNullOrEmpty(Contents) && (Contents.Length + bytesRead > _displayBuffer))
                    {
                        var skipLength = (_displayBuffer - bytesRead) < Contents.Length ? 0 : _displayBuffer - bytesRead;
                        var contentBytes = Encoding.UTF8.GetBytes(Contents).Skip(skipLength).ToArray();
                        trimmedContent = Encoding.UTF8.GetString(contentBytes);
                    }

                    // update the model
                    Contents = trimmedContent + Encoding.UTF8.GetString(newContent);
                    _lastIndex += bytesRead;
                }
            }
            catch (IOException) { }
        }
    }
}
