using System.Collections.ObjectModel;

namespace MusicVideoJukebox
{
    public class MainWindowDesignTimeViewModel : BaseViewModel
    {
        public double Volume { get; set; } = 0.5;
        public ObservableCollection<string> VideoFiles { get; set; } = new ObservableCollection<string> { "File1",
            "File 2 abcdefghijklmnopqrstuvwxy                              z" };
    }
}
