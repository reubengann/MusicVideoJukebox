using MusicVideoJukebox.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox
{
    public class MainWindowDesignTimeViewModel : BaseViewModel
    {
        public double Volume { get; set; } = 0.5;
        public ObservableCollection<string> VideoFiles { get; set; } = new ObservableCollection<string> { "File1",
            "File 2 abcdefghijklmnopqrstuvwxy                              z" };

        public static VideoInfoDesignTimeViewModel InfoViewModel => new();

        public ICommand? PrevCommand => null;
        public ICommand? NextCommand => null;
        public ICommand? PlayPauseCommand => null;
        public ICommand? SettingsCommand => null;

        public int SelectedIndex => 0;

        public List<string> PlaylistNames => new List<string> { "Main" };

        public int SelectedPlaylistIndex => 0;

        public bool ShowPlay => false;

        public bool ShowPause => true;

        public double VideoLengthSeconds => 121.0;

        public double VideoPositionTime { get => 57.1; set { } }
    }
}
