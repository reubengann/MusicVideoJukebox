using MusicVideoJukebox.Core.ViewModels;
using System.Windows;

namespace MusicVideoJukebox.Views
{
    /// <summary>
    /// Interaction logic for PlaylistDetailsEditDialog.xaml
    /// </summary>
    public partial class PlaylistDetailsEditDialog : Window
    {
        private readonly PlaylistDetailsEditDialogViewModel vm;

        public PlaylistDetailsEditDialog(PlaylistDetailsEditDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.RequestedClose += Vm_RequestClose;
            this.vm = vm;
        }

        private void Vm_RequestClose()
        {
            Close();
        }
    }
}
