using System.Windows;

namespace MusicVideoJukebox
{
    public partial class SettingsWindow : Window, ISettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public bool Result { get; set; }

        void ISettingsWindow.ShowDialog()
        {
            ShowDialog();
        }
    }
}
