using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeThreadDispatcher : IUiThreadDispatcher
    {
        public void Invoke(Action action)
        {
            action();
        }
    }
}