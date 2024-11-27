using MusicVideoJukebox.Core.UserInterface;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeInterfaceFader : IFadesWhenInactive
    {
        public bool FadingEnabled = true;

        public void DisableFading()
        {
            FadingEnabled = false;
        }

        public void EnableFading()
        {
            FadingEnabled = true;
        }
    }
}