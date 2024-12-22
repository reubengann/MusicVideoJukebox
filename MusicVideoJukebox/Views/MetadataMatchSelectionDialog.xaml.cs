using MusicVideoJukebox.Core.ViewModels;
using System.Windows;

namespace MusicVideoJukebox.Views
{
    /// <summary>
    /// Interaction logic for MetadataMatchSelectionDialog.xaml
    /// </summary>
    public partial class MetadataMatchSelectionDialog : Window
    {
        public MetadataMatchSelectionDialog(MatchDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
