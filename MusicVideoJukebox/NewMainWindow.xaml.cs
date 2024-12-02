using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Core.ViewModels;
using System.Windows;

namespace MusicVideoJukebox
{
    public partial class NewMainWindow : Window
    {
        private readonly NewMainWindowViewModel vm;
        private readonly InterfaceFader interfaceFader;

        public NewMainWindow(NewMainWindowViewModel vm, INavigationService navigationService)
        {
            InitializeComponent();
            this.vm = vm;
            interfaceFader = new InterfaceFader(Sidebar, OpacityProperty);
            vm.Initialize(interfaceFader);
            navigationService.Initialize(interfaceFader);
            DataContext = vm;
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            interfaceFader.UserInteracted();
        }
    }
}
