namespace MusicVideoJukebox.Core.UserInterface
{
    public class VisibilityChangedEventArgs : EventArgs
    {
        public bool IsVisible { get; }

        public VisibilityChangedEventArgs(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }

    public interface IFadesWhenInactive
    {
        event EventHandler<VisibilityChangedEventArgs>? VisibilityChanged;

        void EnableFading();
        void DisableFading();
        void UserInteracted();
    }
}
