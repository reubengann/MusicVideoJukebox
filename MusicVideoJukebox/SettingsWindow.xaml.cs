using System.Windows;
using System.Windows.Controls;

namespace MusicVideoJukebox
{
    public partial class SettingsWindow : Window, ISettingsWindow
    {

        readonly SettingsWindowViewModel vm;

        public SettingsWindow(VideoLibraryStore videoLibraryStore)
        {
            InitializeComponent();
            Loaded += SettingsWindow_Loaded;
            vm = new SettingsWindowViewModel(videoLibraryStore.VideoLibrary.Folder);
            DataContext = vm;
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

        private bool handleSelection = true;
        private void PlaylistDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // TODO: this does not account for the fact that if you try to do this twice on the same item, it will start editing that item's name :(
            if (handleSelection && vm.HasSettingsToSave)
            {
                MessageBoxResult result = MessageBox.Show
                        ("There are unsaved changes to the playlist. Discard?", "Discard changes?", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    DataGrid combo = (DataGrid)sender;
                    handleSelection = false;
                    combo.SelectedItem = e.RemovedItems[0];
                    return;
                }
            }
            handleSelection = true;
        }
    }
}
