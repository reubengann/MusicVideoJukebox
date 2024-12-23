using MusicVideoJukebox.Core;
using System;

namespace MusicVideoJukebox.Impls
{
    public class WpfUiThreadDispatcher : IUiThreadDispatcher
    {
        public void Invoke(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }
    }
}
