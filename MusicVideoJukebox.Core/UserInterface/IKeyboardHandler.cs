using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicVideoJukebox.Core.UserInterface
{
    public interface IKeyboardHandler
    {
        void NextTrackPressed();
        void PrevTrackPressed();
        void PlayPauseKeyPressed();
    }
}
