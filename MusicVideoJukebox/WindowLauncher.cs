using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Navigation;
using MusicVideoJukebox.Views;
using System;

namespace MusicVideoJukebox
{
    public class WindowLauncher : IWindowLauncher
    {
        public AddLibraryDialogResult LaunchAddLibraryDialog()
        {
            var dialog = new AddLibraryDialog();
            var result = dialog.ShowDialog();
            if (result == true)
            {
                var vm = dialog.DataContext as AddLibraryDialogViewModel;
                ArgumentNullException.ThrowIfNull(vm);
                return new AddLibraryDialogResult { Accepted = true, Path = vm.Path, Name = vm.Name };
            }
            return new AddLibraryDialogResult { Accepted = false };
        }
    }
}
