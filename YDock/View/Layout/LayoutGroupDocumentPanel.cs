using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDock.Interface;
using YDock.Enum;
using System.Windows;
using YDock.Model;

namespace YDock.View
{
    public class LayoutGroupDocumentPanel : LayoutGroupPanel
    {
        public LayoutGroupDocumentPanel()
        {
            
        }

        public override void AttachChild(IDockView child, AttachMode mode, int index)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("index out of range!");
            if (!_AssertMode(mode)) throw new ArgumentException("mode is illegal!");

            if (Count == 1)
            {
                ILayoutSize size = Children[0] as ILayoutSize;
                size.DesiredWidth = ActualWidth;
                size.DesiredHeight = ActualHeight;
            }

            if (child is LayoutDocumentGroupControl
                || child is LayoutGroupDocumentPanel
                || mode == AttachMode.Left_WithSplit
                || mode == AttachMode.Top_WithSplit
                || mode == AttachMode.Right_WithSplit
                || mode == AttachMode.Bottom_WithSplit)
                _AttachWithSplit(child, mode, index);
            else
            {
                if (IsRootPanel)
                    AttachToRootPanel(child, mode);
                else
                {
                    var parent = Parent as LayoutGroupPanel;
                    switch (parent.Direction)
                    {
                        case Direction.LeftToRight:
                            if (mode == AttachMode.Left || mode == AttachMode.Right)
                            {
                                if (child is LayoutGroupPanel)
                                    _AttachSubPanel((child as LayoutGroupPanel), index);
                                else _AttachChild(child, index);
                            }
                            else
                            {
                                int _index = parent.IndexOf(this);
                                parent._DetachChild(this);
                                var pparent = new LayoutGroupPanel() { Direction = Direction.UpToDown };
                                parent._AttachChild(pparent, _index);
                                pparent._AttachChild(this, 0);
                                pparent._AttachChild(child, mode == AttachMode.Top ? 0 : 1);
                            }
                            break;
                        case Direction.UpToDown:
                            if (mode == AttachMode.Top || mode == AttachMode.Bottom)
                            {
                                if (child is LayoutGroupPanel)
                                    _AttachSubPanel((child as LayoutGroupPanel), index);
                                else _AttachChild(child, index);
                            }
                            else
                            {
                                int _index = parent.IndexOf(this);
                                parent._DetachChild(this);
                                var pparent = new LayoutGroupPanel() { Direction = Direction.LeftToRight };
                                parent._AttachChild(pparent, _index);
                                pparent._AttachChild(this, 0);
                                pparent._AttachChild(child, mode == AttachMode.Left ? 0 : 1);
                            }
                            break;
                    }
                }
            }
        }

        public override void DetachChild(IDockView child, bool force = true)
        {
            if (!(child is LayoutDocumentGroupControl))
                throw new InvalidOperationException("not support Operation!");
            else
            {
                _DetachChild(child);
                if (Count < 2)
                    Direction = Direction.None;
                if (Count == 1)
                {
                    if (DockViewParent == null)
                    {
                        var _child = Children[0];
                        var wnd = Parent as ILayoutViewParent;
                        wnd.DetachChild(this, false);
                        Children.Clear();
                        wnd.AttachChild(_child as IDockView, AttachMode.None, 0);
                    }
                }
            }
        }

        private void _AttachWithSplit(IDockView child, AttachMode mode, int index)
        {
            if (child is LayoutDocumentGroupControl)
            {
                if (Direction == Direction.None)
                    Direction = (mode == AttachMode.Left_WithSplit || mode == AttachMode.Right_WithSplit) ? Direction.LeftToRight : Direction.UpToDown;
                _AttachChild(child, index);
            }

            if (child is AnchorSideGroupControl)
            {
                var model = (child as AnchorSideGroupControl).Model as LayoutGroup;
                var _children = new List<IDockElement>(model.Children);
                model.Dispose();
                var group = new LayoutDocumentGroup(DockManager);
                foreach (var _child in _children)
                    group.Attach(_child);
                var ctrl = new LayoutDocumentGroupControl(group);
                _AttachChild(ctrl, index);
            }

            if (child is LayoutGroupDocumentPanel
                || child is LayoutGroupPanel)
            {
                var _children = new List<IDockView>((child as LayoutGroupPanel).Children.OfType<IDockView>());
                _children.Reverse();
                (child as LayoutGroupPanel).Children.Clear();
                foreach (var _child in _children)
                    _AttachWithSplit(_child as IDockView, mode, index);
            }

            if (child is IDisposable)
                (child as IDisposable).Dispose();
        }

        internal override bool _AssertMode(AttachMode mode)
        {
            return mode == AttachMode.Left
                || mode == AttachMode.Top
                || mode == AttachMode.Right
                || mode == AttachMode.Bottom
                || mode == AttachMode.Left_WithSplit
                || mode == AttachMode.Top_WithSplit
                || mode == AttachMode.Right_WithSplit
                || mode == AttachMode.Bottom_WithSplit;
        }
    }
}