using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SwiftTailer.Wpf.Commands
{
    public class PingSelectionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var tb = parameter as TextBox;
            if (tb == null)
            {
                Trace.WriteLine("Did not get a textbox to work with.", ToString());
                return;
            }

            var selectedText = tb.SelectedText;

            var ipAddress = GetIpAddress(selectedText);
            var message = $"Unable to ping {selectedText}!";
            var succeeded = false;

            try
            {
                if(ipAddress == null)
                    throw new Exception($"Could not resolve {selectedText}");

                var pingSender = new Ping();
                var reply = pingSender.Send(ipAddress);

                
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    succeeded = true;
                    message = string.Format(
                        "Successfully pinged {0} with a round trip of {1}ms",
                        reply.Address,
                        reply.RoundtripTime);
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            finally
            {
                MessageBox.Show(
                    message,
                    "Ping Result",
                    MessageBoxButton.OK,
                    succeeded ? MessageBoxImage.Information : MessageBoxImage.Error);
            }
        }

        private static IPAddress GetIpAddress(string thing)
        {
            IPAddress ipAddress;

            // try to parse ip
            if (IPAddress.TryParse(thing, out ipAddress))
                return ipAddress;

            // try to resolve host to ip
            IPHostEntry hostEntry = null;

            try
            {
                hostEntry = Dns.GetHostEntry(thing);
                if (hostEntry == null || !hostEntry.AddressList.Any()) return null;
            }
            catch { /* if host doesn't resolve, it throw an exception that we don't actually care about */ }

            var firstResult = hostEntry?.AddressList[0];
            return firstResult;
        }
    }
}