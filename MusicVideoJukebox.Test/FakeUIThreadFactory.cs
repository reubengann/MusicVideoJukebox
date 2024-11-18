using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeUIThreadFactory : IUIThreadTimerFactory
    {
        public IUIThreadTimer Create(TimeSpan interval)
        {
            throw new NotImplementedException();
        }
    }
}
