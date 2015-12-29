using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using SwiftTailer.Wpf.Data;
using SwiftTailer.Wpf.Models;

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
                        if (!File.Exists(log.Filename))
                            continue;

                        progressProvider.ProgressText = $"Processing {log.Filename}...";

                        // create the entry
                        var entry = zip.CreateEntry(Path.GetFileName(log.Filename), CompressionLevel.Fastest);

                        // fill the entry
                        using (var entryStream = entry.Open())
                        {
                            using (
                                var logFileStream = File.Open(log.Filename, FileMode.Open, FileAccess.Read,
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