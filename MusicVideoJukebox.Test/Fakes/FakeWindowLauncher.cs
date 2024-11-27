using MusicVideoJukebox.Core.Navigation;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeWindowLauncher : IWindowLauncher
    {
        public AddLibraryDialogResult ToReturn {  get; set; } = new AddLibraryDialogResult { Accepted = false };

        public AddLibraryDialogResult LaunchAddLibraryDialog()
        {
            return ToReturn;
        }
    }
}