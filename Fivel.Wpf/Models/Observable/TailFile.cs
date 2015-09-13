using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Fivel.Wpf.Data;
using Provausio.Common.Portable;

namespace Fivel.Wpf.Models.Observable
{
    public class TailFile : ModelBase
    {
        private readonly long _displayBuffer;
        private readonly TimeSpan _interval;
        private CancellationTokenSource _cts;
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
            _displayBuffer = Settings.DisplayBufferSize * 0x400;
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
            _cts = new CancellationTokenSource();
            if (File.Exists(LogInfo.Location))
            {
                await Task.Run(async () =>
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        Contents = await GetUpdates();
                        await Task.Delay(Settings.PollingInterval);
                    }
                }, _cts.Token);
            }
            else
            {
                await Task.Run(() =>
                {
                    Trace.WriteLine($"{LogInfo.Location} was not found.");
                    Contents = $"File not found: {LogInfo.Location}";
                });
            }

            Trace.WriteLine($"Stopped tailing {LogInfo.Alias}");
        }
        
        private async Task<string> GetUpdates()
        {
            try
            {
                using (var fs = new FileStream(LogInfo.Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length == 0 || !fs.CanRead || fs.Length == _lastIndex)
                        return Contents; // no change

                    // avoid reading the entire file on startup
                    var startAt = _lastIndex;
                    if (startAt == 0 && fs.Length > _displayBuffer)
                    {
                        startAt = fs.Length - _displayBuffer;
                        Trace.WriteLine(
                            $"{LogInfo.Alias} file was larger than buffer ({_displayBuffer}). Starting at i={startAt} instead of beginning.");
                    }

                    var contentLength = fs.Length - startAt;
                    if (contentLength < 0)
                    {
                        // trying to track some weird overflow case
                        Trace.WriteLine($"!!!!!!{LogInfo.Location} Length: {fs.Length} startAt: {startAt}");
                        return Contents;
                    }

                    var newContent = new byte[contentLength];
                    Trace.WriteLine($"This chunk will be {newContent.Length} bytes.");

                    fs.Seek(startAt, SeekOrigin.Begin);

                    await fs.ReadAsync(newContent, 0, newContent.Length);

                    // trim the buffer off the front
                    Contents += Encoding.UTF8.GetString(newContent);
                    var trimmedContent = string.Empty;
                    if (Contents.Length > _displayBuffer)
                    {
                        var contentBytes = Encoding.UTF8.GetBytes(Contents);
                        var newBytes = new byte[_displayBuffer];
                        Array.Copy(contentBytes, contentBytes.Length - _displayBuffer, newBytes, 0, _displayBuffer);
                        trimmedContent = Encoding.UTF8.GetString(newBytes);
                    }

                    // update the model
                    var c = trimmedContent + Encoding.UTF8.GetString(newContent);
                    Trace.WriteLine($"Content is now {Contents.Length} characters.");
                    _lastIndex = startAt;
                    _lastIndex = fs.Position;
                    return c;
                }
            }
            catch (IOException)
            {
                return Contents;
            }
        }

        #region -- Old Style Threading --

        
        #endregion
    }
}
