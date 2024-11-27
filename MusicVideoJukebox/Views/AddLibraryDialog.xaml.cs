using MusicVideoJukebox.Core;
using System.Windows;

namespace MusicVideoJukebox.Views
{
    /// <summary>
    /// Interaction logic for AddLibraryDialog.xaml
    /// </summary>
    public partial class AddLibraryDialog : Window
    {
        private readonly AddLibraryDialogViewModel vm;

        public AddLibraryDialog(AddLibraryDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.RequestClose += Vm_RequestClose;
            Closed += (s, e) => vm.RequestClose -= Vm_RequestClose;
            this.vm = vm;
        }

        private void Vm_RequestClose(bool result)
        {
            DialogResult = result;
            Close();
        }
    }
}
