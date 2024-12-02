using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeUiThreadTimer : IUIThreadTimer
    {
        public event EventHandler? Tick;
        public bool Started = false;
        public bool Stopped = false;

        public void Start()
        {
            Started = true;
        }

        public void Stop()
        {
            Stopped = true;
        }

        public void Trigger()
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }
    }

    internal class FakeUIThreadFactory : IUIThreadTimerFactory
    {
        public List<FakeUiThreadTimer> ToReturn = [];
        int index = 0;

        public IUIThreadTimer Create(TimeSpan interval)
        {
            var foo = ToReturn[index];
            index ++;
            return foo;
        }
    }
}
