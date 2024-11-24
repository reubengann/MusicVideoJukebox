using MusicVideoJukebox.Core;
using System.Windows;

namespace MusicVideoJukebox
{
    /// <summary>
    /// Interaction logic for NewMainWindow.xaml
    /// </summary>
    public partial class NewMainWindow : Window
    {
        private readonly NewMainWindowViewModel vm;

        public NewMainWindow(NewMainWindowViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
            DataContext = vm;
        }
    }
}
