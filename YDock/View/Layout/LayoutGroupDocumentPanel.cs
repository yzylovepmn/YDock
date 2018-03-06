using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDock.Interface;
using YDock.Enum;
using System.Windows;

namespace YDock.View
{
    public class LayoutGroupDocumentPanel : LayoutGroupPanel
    {
        public LayoutGroupDocumentPanel()
        {
            
        }

        public override void AttachChild(IDockView child, int index)
        {
            if (child is LayoutDocumentGroupControl)
                _AttachChild(child, index);
            else if (child is LayoutGroupDocumentPanel)
            {
                var panel = (child as LayoutGroupDocumentPanel);
                Direction = panel.Direction;
                for (int i = panel.Children.Count - 1; i >= 0; i -= 2)
                    _AttachChild(panel.Children[i] as IDockView, index);
            }
            else if (child is LayoutGroupPanel)
                throw new InvalidOperationException("not support Operation!");
            else
            {
                var _child = child as AnchorSideGroupControl;
                var dockManager = DockManager;
                //首先从Parent移除此Panel
                DockManager.LayoutRootPanel.DetachChild(this);
                var pparent = new LayoutGroupPanel() { Direction = (child.Model.Side == DockSide.Top || child.Model.Side == DockSide.Bottom) ? Direction.UpToDown : Direction.LeftToRight };
                if (child.Model.Side == DockSide.Top
                    || child.Model.Side == DockSide.Left)
                {
                    pparent._AttachChild(this, 0);
                    pparent._AttachChild(child, 0);
                }
                else
                {
                    pparent._AttachChild(child, 0);
                    pparent._AttachChild(this, 0);
                }
                //从Parent加入此Panel
                dockManager.LayoutRootPanel.AttachChild(pparent, 0);
            }
        }

        public override void DetachChild(IDockView child)
        {
            if (!(child is LayoutDocumentGroupControl))
                throw new InvalidOperationException("not support Operation!");
            else
            {
                if (Children.Count < 2)
                    return;
                _DetachChild(child);
            }
        }
    }
}