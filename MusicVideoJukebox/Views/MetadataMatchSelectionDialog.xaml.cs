using MusicVideoJukebox.Core.ViewModels;
using System.Windows;

namespace MusicVideoJukebox.Views
{
    public partial class MetadataMatchSelectionDialog : Window
    {
        private readonly MatchDialogViewModel vm;

        public MetadataMatchSelectionDialog(MatchDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += MetadataMatchSelectionDialog_Loaded;
            this.vm = vm;
            vm.RequestClose += Vm_RequestClose;
        }

        private void Vm_RequestClose()
        {
            Close();
        }

        private async void MetadataMatchSelectionDialog_Loaded(object sender, RoutedEventArgs e)
        {
            await vm.Initialize();
        }
    }
}
