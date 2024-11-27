using MusicVideoJukebox.Core.Navigation;

namespace MusicVideoJukebox.Test
{
    internal class FakeWindowLauncher : IWindowLauncher
    {
        public AddLibraryDialogResult LaunchAddLibraryDialog()
        {
            throw new NotImplementedException();
        }
    }
}