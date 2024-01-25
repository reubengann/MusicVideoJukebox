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
            vm = new SettingsWindowViewModel(videoLibraryStore);
            DataContext = vm;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (vm.HasSettingsToSave)
            {
                MessageBoxResult result = MessageBox.Show
                        ("There are unsaved changes to the playlist. Discard?", "Discard changes?", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            Close();
        }
    }
}
