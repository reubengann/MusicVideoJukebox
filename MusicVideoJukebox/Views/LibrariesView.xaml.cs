using System.Windows.Controls;

namespace MusicVideoJukebox.Views
{
    /// <summary>
    /// Interaction logic for LibrariesView.xaml
    /// </summary>
    public partial class LibrariesView : UserControl
    {
        public LibrariesView()
        {
            InitializeComponent();
        }

        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //if (sender is FrameworkElement element &&
            //element.DataContext is ItemViewModel hoveredViewModel
            //&& DataContext is LauncherViewModel launcherViewModel)
            //{
            //    launcherViewModel.Hovered(hoveredViewModel);
            //}
        }
    }
}
