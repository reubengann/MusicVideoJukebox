using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistDetailsEditDialogViewModel : BaseViewModel
    {
        private readonly Playlist playlist;

        public string PlaylistName { get => playlist.PlaylistName; set => SetUnderlyingProperty(playlist.PlaylistName, value, v => { playlist.PlaylistName = v; }); }
        public string PlaylistDescription { get; set; }
        public string PlaylistIcon { get; set; }

        public ICommand SelectIconCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public bool Accepted { get; set; } = false;

        public PlaylistDetailsEditDialogViewModel(Playlist playlist)
        {
            // Make a copy
            this.playlist = new Playlist { PlaylistId = playlist.PlaylistId, PlaylistName = playlist.PlaylistName };
            SelectIconCommand = new DelegateCommand(SelectIcon);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            throw new NotImplementedException();
        }

        private void Save()
        {
            throw new NotImplementedException();
        }

        private void SelectIcon()
        {
            throw new NotImplementedException();
        }
    }
}
