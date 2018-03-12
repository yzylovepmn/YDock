using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace YDock.Interface
{
    public interface IMenuItem : IDockItem
    {
        void SetContextMenu(ContextMenu menu);
    }
}