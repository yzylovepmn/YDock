using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YDock.Interface
{
    public interface IDockView
    {
        IDockModel Model { get; set; }
        IDockView DockViewParent { get; }
    }
}