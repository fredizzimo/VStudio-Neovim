using Microsoft.VisualStudio.PlatformUI.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VStudioNeovim.UI
{
    class DocumentWindow
    {
        public ViewElement ViewElement { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
    }
}
