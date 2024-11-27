using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicVideoJukebox.Core
{
    public class AddLibraryDialogViewModel : BaseViewModel
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
    }
}
