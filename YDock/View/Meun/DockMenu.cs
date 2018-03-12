using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using YDock.Interface;

namespace YDock.View
{
    public class DockMenu : ContextMenu
    {
        public DockMenu(IMenuItem targetObj)
        {
            _targetObj = targetObj;
            _targetObj.SetContextMenu(this);
        }

        private IMenuItem _targetObj;
        public IMenuItem TargetObj { get { return _targetObj; } }
    }
}