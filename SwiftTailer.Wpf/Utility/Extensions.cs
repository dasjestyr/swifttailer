using System;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace SwiftTailer.Wpf.Utility
{
    public static class Extensions
    {
        public static T FindAncestor<T>(this DependencyObject obj) 
            where T : DependencyObject
        {
            // done as a loop to avoid explicit recursion
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(obj);
                if (parent == null) return null;

                var parentT = parent as T;
                if (parentT != null) return parentT;
                obj = parent;
            }
        }

        public static T FindDescendant<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            // done as a loop to avoid explicit recursion
            while (true)
            {
                var parent = VisualTreeHelper.GetChild(obj, 0);
                if (parent == null) return null;

                var parentT = parent as T;
                if (parentT != null) return parentT;
                obj = parent;
            }
        }
    }

    public static class MailUtility
    {
        //Extension method for MailMessage to save to a file on disk
        public static void Save(this MailMessage message, string filename, bool addUnsentHeader = true)
        {
            using (var filestream = File.Open(filename, FileMode.Create))
            {
                if (addUnsentHeader)
                {
                    var binaryWriter = new BinaryWriter(filestream);
                    //Write the Unsent header to the file so the mail client knows this mail must be presented in "New message" mode
                    binaryWriter.Write(System.Text.Encoding.UTF8.GetBytes("X-Unsent: 1" + Environment.NewLine));
                }

                var assembly = typeof(SmtpClient).Assembly;
                var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

                // Get reflection info for MailWriter contructor
                var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

                // Construct MailWriter object with our FileStream
                var mailWriter = mailWriterContructor.Invoke(new object[] { filestream });

                // Get reflection info for Send() method on MailMessage
                var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { mailWriter, true, true }, null);

                // Finally get reflection info for Close() method on our MailWriter
                var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
            }
        }
    }
}
