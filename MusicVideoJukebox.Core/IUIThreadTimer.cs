namespace MusicVideoJukebox.Core
{
    public interface IUIThreadTimer
    {
        event EventHandler? Tick;

        void Start();
        void Stop();
    }

    public interface IUIThreadTimerFactory
    {
        IUIThreadTimer Create(TimeSpan interval);
    }
}
