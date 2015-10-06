using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.ViewModels;

namespace SwiftTailer.Wpf.Commands
{
    public class ZipTasks
    {
        public static void ZipGroup(LogGroup logGroup, string saveLocation, IProgressProvider progressProvider)
        {
            Trace.WriteLine("task started");
            progressProvider.ProgressBarValue = 0;
            using (var ms = new FileStream(saveLocation, FileMode.CreateNew))
            {
                // create the archive to go into the file
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create))
                {
                    var progressFactor = 100 / logGroup.Logs.Count;

                    // add each log to the archive
                    foreach (var log in logGroup.Logs)
                    {
                        if (!File.Exists(log.FullPath))
                            continue;

                        progressProvider.ProgressText = $"Processing {log.FullPath}...";

                        // create the entry
                        var entry = zip.CreateEntry(Path.GetFileName(log.FullPath), CompressionLevel.Fastest);

                        // fill the entry
                        using (var entryStream = entry.Open())
                        {
                            using (
                                var logFileStream = File.Open(log.FullPath, FileMode.Open, FileAccess.Read,
                                    FileShare.ReadWrite))
                            {
                                var fileBytes = new byte[logFileStream.Length];
                                logFileStream.Read(fileBytes, 0, fileBytes.Length);
                                using (var logStream = new MemoryStream(fileBytes))
                                {
                                    logStream.CopyTo(entryStream);
                                    progressProvider.ProgressBarValue += progressFactor;
                                }
                            }
                        }
                    }
                }
            }

            progressProvider.ProgressText = string.Empty;
            progressProvider.ProgressBarValue = 0;
            
        }
    }
}