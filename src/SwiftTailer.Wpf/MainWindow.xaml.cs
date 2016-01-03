using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using SwiftTailer.Wpf.Commands;
using SwiftTailer.Wpf.Infrastructure.Messaging;
using SwiftTailer.Wpf.Models;

namespace SwiftTailer.Wpf
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Dispatcher.UnhandledException += DisplayException;
            SourceInitialized += OnSourceInitialized; // Used to hook window behaviors before they fire

            MessageBroker.Subscribe(typeof(WindowFocusedRequestMessage), RequestFocusHandler);

            SetInitialWindowSize();
        }

        private void SetInitialWindowSize()
        {
            PropertyChangedCallback windowSizeCallback =
                (source, args) =>
                {
                    var workArea = (Rect)args.NewValue;
                    Height = workArea.Height;
                    Width = workArea.Width*.7;
                };

            var windowSizeProperty = DependencyProperty.Register("InitialWindowSize", typeof(Rect), typeof(Window),
                                                                 new PropertyMetadata(windowSizeCallback));

            SetResourceReference(windowSizeProperty, SystemParameters.WorkAreaKey);
        }

        private void RequestFocusHandler(DomainEvent args)
        {
            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Application.Current.ShutdownMode == ShutdownMode.OnExplicitShutdown)
            {
                // Cancel the close
                e.Cancel = true;

                // Execute the command that handles minimize window to tray icon behavior
                if (TrayIconViewModel.HideWindowCommand.CanExecute(null))
                {
                    TrayIconViewModel.HideWindowCommand.Execute(null);
                }
            }
            else
            {
                // Business as usual
                base.OnClosing(e);
            }
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            var source = (HwndSource)PresentationSource.FromVisual(this);
            source?.AddHook(HandleMessages);
        }

        private static IntPtr HandleMessages(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // 0x0112 == WM_SYSCOMMAND, 'Window' command message.
            // 0xF020 == SC_MINIMIZE, command to minimize the window.
            if (msg == 0x0112 && ((int)wParam & 0xFFF0) == 0xF020)
            {
                // Cancel the minimize.
                handled = true;

                // Execute the command that handles minimize window to tray icon behavior
                if (TrayIconViewModel.HideWindowCommand.CanExecute(null))
                {
                    TrayIconViewModel.HideWindowCommand.Execute(null);
                }
            }
            else
            {
                handled = false;
            }

            return IntPtr.Zero;
        }

        private static void DisplayException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Global error catch 
            // Doesn't really allow the app to resume, but it's better than logs
            // for now
            // var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "error.log");
            var logPath = Path.Combine(Settings.WorkingDirectory, "error.log");

            using(var sw = File.AppendText(logPath))
            {
                sw.WriteLine($"{DateTime.Now.ToLongDateString()} :: {e.Exception.Message} :: {e.Exception}");
            }

            MessageBox.Show($"e.Exception.Message :: {e.Exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LogList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // The selection gets changed throught it's SelectedItem binding
            // This old school sauce is to deal with the fact that custom 
            // behaviors just weren't having me 

            var lbx = sender as ListBox;
            if (lbx == null) return;

            var vm = DataContext as MainViewModel;
            var tail = vm.SelectedTail;

            if (tail == null) return;

            if(tail.FollowTail)
                lbx.ScrollIntoView(lbx.SelectedItem);
        }

        private void ListBoxItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lbi = sender as ListBoxItem;
            if (lbi == null) return;

            StaticCommands.OpenLogLineCommand.Execute(lbi);
        }
    }
}
