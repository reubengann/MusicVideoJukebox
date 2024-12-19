using MusicVideoJukebox.Core;
using System;
using System.Windows.Threading;

namespace MusicVideoJukebox
{
    public class DispatcherUITimer : IUIThreadTimer
    {
        readonly DispatcherTimer _timer;

        public DispatcherUITimer(TimeSpan interval)
        {
            _timer = new DispatcherTimer
            {
                Interval = interval
            };
            _timer.Tick += (sender, args) => Tick?.Invoke(this, EventArgs.Empty);
        }


        public event EventHandler? Tick;

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }

    public class DispatcherUITimerFactory : IUIThreadTimerFactory
    {
        public IUIThreadTimer Create(TimeSpan interval)
        {
            return new DispatcherUITimer(interval);
        }
    }
}
