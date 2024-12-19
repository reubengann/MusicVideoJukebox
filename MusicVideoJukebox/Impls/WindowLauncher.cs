using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Views;
using System;

namespace MusicVideoJukebox
{
    public class WindowLauncher(IDialogService dialogService, ILibrarySetRepo librarySetRepo) : IWindowLauncher
    {
        private readonly IDialogService dialogService = dialogService;
        private readonly ILibrarySetRepo librarySetRepo = librarySetRepo;

        public AddLibraryDialogResult LaunchAddLibraryDialog()
        {
            var vm = new AddLibraryDialogViewModel(dialogService, librarySetRepo);
            var dialog = new AddLibraryDialog(vm);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                ArgumentNullException.ThrowIfNull(vm);
                return new AddLibraryDialogResult { Accepted = true, Path = vm.FolderPath, Name = vm.LibraryName };
            }
            return new AddLibraryDialogResult { Accepted = false };
        }
    }
}
