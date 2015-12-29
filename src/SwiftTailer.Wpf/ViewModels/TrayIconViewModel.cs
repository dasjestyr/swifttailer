using System;
using System.Windows;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;

namespace SwiftTailer.Wpf.ViewModels
{
    public class TrayIconViewModel
    {
        /// <summary>
        /// Shows a window, if none is already open, otherwise brings existing window to foreground.
        /// </summary>
        public static ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => true,
                    CommandAction = () =>
                    {
                        var mainWindow = Application.Current.MainWindow;

                        // ReSharper disable once InvertIf
                        if (mainWindow == null)
                        {
                            mainWindow = new MainWindow();
                            mainWindow.Show();
                        }
                        mainWindow.Visibility = Visibility.Visible;
                        mainWindow.Activate();
                    }
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public static ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow != null &&
                                           Application.Current.MainWindow.Visibility == Visibility.Visible,
                    CommandAction = () =>
                    {
                        Application.Current.MainWindow.Visibility = Visibility.Collapsed;
                        var currentApp = Application.Current as App;
                        currentApp?.TaskbarIcon.ShowBalloonTip("SwiftTailer", 
                            "Running in background, use tray icon to exit.", BalloonIcon.Info);
                    }
                };
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }
    }


    /// <summary>
    /// Delegate Command
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Func<bool> CanExecuteFunc { get; set; }
        public Action CommandAction { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}