using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YDock
{
    public class DragManager
    {
        public DragManager(DockManager dockManager)
        {
            _dockManager = dockManager;
        }

        private DockManager _dockManager;
        public DockManager DockManager
        {
            get { return _dockManager; }
        }
    }
}