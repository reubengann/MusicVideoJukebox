namespace MusicVideoJukebox
{
    class WindowsSettingsWindowFactory : ISettingsWindowFactory
    {
        public ISettingsWindow Create()
        {
            return new SettingsWindow();
        }
    }
}
