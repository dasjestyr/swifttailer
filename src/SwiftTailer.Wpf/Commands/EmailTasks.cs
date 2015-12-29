using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using SwiftTailer.Wpf.Utility;

namespace SwiftTailer.Wpf.Commands
{
    public class EmailTasks
    {
        public static void SendAttachment(string from, string subject, string body, string attachmentPath)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(from);
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = Path.GetFileName(attachmentPath) ?? subject;

                mailMessage.Attachments.Add(new Attachment(attachmentPath));


                var filename = $"{Path.GetFileName(attachmentPath)}.eml";

                //save the MailMessage to the filesystem
                mailMessage.Save(filename);

                //Open the file with the default associated application registered on the local machine
                Process.Start(filename);
            }
        }
    }
}