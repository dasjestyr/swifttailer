namespace SwiftTailer.Wpf.ViewModels
{
    public interface IProgressProvider
    {
        string ProgressText { get; set; }

        int ProgressBarValue { get; set; }
    }
}