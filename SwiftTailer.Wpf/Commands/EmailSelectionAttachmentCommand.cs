using System;
using System.IO;
using System.Windows.Input;

namespace SwiftTailer.Wpf.Commands
{
    public class EmailSelectionAttachmentCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var content = parameter as string;

            var fileName = $"LogCapture_{DateTime.Now.ToString("yyyyMMdd")}_{DateTime.Now.ToString("HHmmss")}";
            var tempPath = $"{Path.GetTempPath()}\\{fileName}.txt";

            File.WriteAllText(tempPath, content);

            EmailTasks.SendAttachment(
                "someone@domain.com",
                "Log File",
                fileName,
                tempPath);
            
            File.Delete(tempPath);
        }

        public event EventHandler CanExecuteChanged;
    }

    
}
