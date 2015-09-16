using System.Windows.Input;

namespace Fievel.Wpf.Commands
{
    /// <summary>
    /// Use as static commands or as a bucket for when gaining context to the view model is too difficult.
    /// </summary>
    public class StaticCommands
    {
        public static ICommand OpenLogLineCommand { get; set; }
        
        public static ICommand CloseWindowCommand = new CloseWindowCommand();

        public static ICommand DeleteGroupCommand = new DeleteGroupCommand();

        public static ICommand EditGroupCommand = new EditGroupCommand();

        public static ICommand RemoveLogCommand = new RemoveLogFromGroupCommand();
    }
}