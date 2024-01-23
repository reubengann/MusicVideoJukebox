using System.Windows;

namespace MusicVideoJukebox
{
    public partial class SettingsWindow : Window, ISettingsWindow
    {

        SettingsWindowViewModel vm;

        public SettingsWindow(VideoLibraryStore videoLibraryStore)
        {
            InitializeComponent();
            Loaded += SettingsWindow_Loaded;
            vm = new SettingsWindowViewModel(videoLibraryStore.VideoLibrary.Folder);
        }

        private async void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await vm.LoadAllMetadataAsync();
        }

        public bool Result { get; set; }

        void ISettingsWindow.ShowDialog()
        {
            ShowDialog();
        }
    }
}
