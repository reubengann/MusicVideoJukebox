using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeUiThreadTimer : IUIThreadTimer
    {
        public event EventHandler? Tick;
        bool Started = false;
        bool Stopped = false;

        public void Start()
        {
            Started = true;
        }

        public void Stop()
        {
            Stopped = true;
        }
    }

    internal class FakeUIThreadFactory : IUIThreadTimerFactory
    {
        public IUIThreadTimer Create(TimeSpan interval)
        {
            return new FakeUiThreadTimer();
        }
    }
}
