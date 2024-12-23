namespace MusicVideoJukebox.Core
{
    public interface IUiThreadDispatcher
    {
        void Invoke(Action action);
    }
}
