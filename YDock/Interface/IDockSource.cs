using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace YDock.Interface
{
    public interface IDockSource
    {
        string Header { get; }
        ImageSource Icon { get; }
        IDockControl DockControl { get; set; }
    }

    public interface IDockDocSource : IDockSource
    {
        bool AllowClose();
        string FullFileName { get; }
    }
}
