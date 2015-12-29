namespace SwiftTailer.Wpf.Models
{
    public interface IProgressProvider
    {
        string ProgressText { get; set; }

        int ProgressBarValue { get; set; }
    }
}