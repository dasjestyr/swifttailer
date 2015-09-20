using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Windows.Input;
using SwiftTailer.Wpf.Utility;

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

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress("someone@yourdomain.com");
                mailMessage.Subject = "Log File";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = fileName;

                mailMessage.Attachments.Add(new Attachment(tempPath));


                var filename = $"{fileName}.eml";

                //save the MailMessage to the filesystem
                mailMessage.Save(filename);

                //Open the file with the default associated application registered on the local machine
                Process.Start(filename);
            }
            
            File.Delete(tempPath);
        }

        public event EventHandler CanExecuteChanged;
    }

    
}
