using SwiftTailer.Wpf.Models;

namespace SwiftTailer.Wpf.Pages
{
    public interface ITailerWindowFactory
    {
        MainWindow Create();
    }

    public class TailerWindowFactory : ITailerWindowFactory
    {
        public MainWindow Create()
        {
            // initialize a new tailer window
            var tailerWindow = new MainWindow {DataContext = new MainViewModel()};
            
            return tailerWindow;
        }
    }
}
