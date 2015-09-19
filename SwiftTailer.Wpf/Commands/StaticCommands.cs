using System.Windows.Input;

namespace SwiftTailer.Wpf.Commands
{
    /// <summary>
    /// Use as static commands or as a bucket for when gaining context to the view model is too difficult.
    /// </summary>
    public class StaticCommands
    {
        public static ICommand OpenLogLineCommand = new OpenLogLineCommand();
        
        public static ICommand CloseWindowCommand = new CloseWindowCommand();

        public static ICommand DeleteGroupCommand = new DeleteGroupCommand();

        public static ICommand EditGroupCommand = new EditGroupCommand();

        public static ICommand RemoveLogCommand = new RemoveLogFromGroupCommand();

        public static ICommand FeatureStubCommand = new FeatureStubCommand();

        public static ICommand ExportLogConfigCommand = new ExportLogConfigCommand();

        public static ICommand ImportLogConfigCommand = new ImportLogConfigCommand();

        public static ICommand OpenPathInExplorerCommand = new OpenPathInExplorerCommand();

        public static ICommand OpenLogConfigInExplorerCommand = new OpenLogConfigInExplorerCommand();
    }
}