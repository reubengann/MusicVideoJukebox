using System.ComponentModel;

namespace MusicVideoJukebox
{
    class WindowSizeProvider : INotifyPropertyChanged
    {
        private double _windowWidth;
        private double _windowHeight;

        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                OnPropertyChanged(nameof(WindowWidth));
            }
        }

        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                OnPropertyChanged(nameof(WindowHeight));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
