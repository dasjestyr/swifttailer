using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SwiftTailer.Wpf.Infrastructure
{
    public class DialogCoordinator : IDialogCoordinator
    {
        /// <summary>
        /// Gets the default instance if the dialog coordinator, which can be injected into a view model.
        /// 
        /// </summary>
        public static readonly DialogCoordinator Instance = new DialogCoordinator();

        public Task<string> ShowInputAsync(object context, string title, string message, MetroDialogSettings metroDialogSettings = null)
        {
            return GetMetroWindow(context).ShowInputAsync(title, message, metroDialogSettings);
        }

        public Task<LoginDialogData> ShowLoginAsync(object context, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, LoginDialogSettings settings = null)
        {
            return GetMetroWindow(context).ShowLoginAsync(title, message, settings);
        }

        public Task<MessageDialogResult> ShowMessageAsync(object context, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return GetMetroWindow(context).ShowMessageAsync(title, message, style, settings);
        }

        public Task<ProgressDialogController> ShowProgressAsync(object context, string title, string message, bool isCancelable = false, MetroDialogSettings settings = null)
        {
            return GetMetroWindow(context).ShowProgressAsync(title, message, isCancelable, settings);
        }

        public Task ShowMetroDialogAsync(object context, BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            return GetMetroWindow(context).ShowMetroDialogAsync(dialog, settings);
        }

        public Task HideMetroDialogAsync(object context, BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            return GetMetroWindow(context).HideMetroDialogAsync(dialog, settings);
        }

        public Task<TDialog> GetCurrentDialogAsync<TDialog>(object context) where TDialog : BaseMetroDialog
        {
            return GetMetroWindow(context).GetCurrentDialogAsync<TDialog>();
        }

        private static MetroWindow GetMetroWindow(object context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!DialogParticipation.IsRegistered(context))
                throw new InvalidOperationException("Context is not registered. Consider using DialogParticipation.Register in XAML to bind in the DataContext.");

            var metroWindow = Window.GetWindow(DialogParticipation.GetAssociation(context)) as MetroWindow;

            if (metroWindow == null)
                throw new InvalidOperationException("Control is not inside a MetroWindow.");

            return metroWindow;
        }
    }
}
