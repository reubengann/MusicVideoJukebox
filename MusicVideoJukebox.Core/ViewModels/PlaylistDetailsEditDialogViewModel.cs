using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.UserInterface;
using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistDetailsEditDialogViewModel : BaseViewModel
    {
        private readonly Playlist playlist;
        private readonly IDialogService dialogService;
        private readonly IImageScalerService imageScalerService;
        private readonly LibraryStore libraryStore;

        public string PlaylistName { get => playlist.PlaylistName; set => SetUnderlyingProperty(playlist.PlaylistName, value, v => { playlist.PlaylistName = v; }); }
        public string? PlaylistDescription { get => playlist.Description; set => SetUnderlyingProperty(playlist.Description, value, v => { playlist.Description = v; }); }
        public string PlaylistIcon { 
            get => GetPath(); 
            set => SetUnderlyingProperty(playlist.ImagePath, value, v => { playlist.ImagePath = v; }); 
        }

        public ICommand SelectIconCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public event Action? RequestedClose;

        public bool Accepted { get; set; } = false;
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public Playlist NewPlaylist => playlist;

        string GetPath()
        {
            if (libraryStore.CurrentState == null
                || libraryStore.CurrentState.LibraryPath == null
                || playlist.ImagePath == null) return "/Images/image_off.png";
            return Path.Combine(libraryStore.CurrentState.LibraryPath, playlist.ImagePath);
        }

        public PlaylistDetailsEditDialogViewModel(Playlist playlist, IDialogService dialogService, IImageScalerService imageScalerService, LibraryStore libraryStore)
        {
            // Make a copy
            this.playlist = new Playlist { PlaylistId = playlist.PlaylistId, PlaylistName = playlist.PlaylistName, Description = playlist.Description, ImagePath = playlist.ImagePath };
            SelectIconCommand = new DelegateCommand(SelectIcon);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            this.dialogService = dialogService;
            this.imageScalerService = imageScalerService;
            this.libraryStore = libraryStore;
        }

        private void Cancel()
        {
            Accepted = false;
            RequestedClose?.Invoke();
        }

        private void Save()
        {
            Accepted = true;
            RequestedClose?.Invoke();
        }

        private async void SelectIcon()
        {
            var pickResult = dialogService.PickSingleFile("PNG Files (*.png)|*.png");
            if (!pickResult.Accepted) return;
            ArgumentNullException.ThrowIfNull(pickResult.SelectedFile);
            ArgumentNullException.ThrowIfNull(libraryStore.CurrentState.LibraryPath);

            var newFilename = Guid.NewGuid().ToString() + ".png";
            var writtenFilePath = await imageScalerService.ScaleImage(libraryStore.CurrentState.LibraryPath, pickResult.SelectedFile, newFilename);
            if (writtenFilePath == null)
            {
                dialogService.ShowError("Unable to scale image!");
                return;
            }
            PlaylistIcon = writtenFilePath;
        }
    }
}
