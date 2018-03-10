using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class LayoutDocumentGroupControl : BaseGroupControl
    {
        static LayoutDocumentGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentGroupControl), new FrameworkPropertyMetadata(typeof(LayoutDocumentGroupControl)));
            FocusableProperty.OverrideMetadata(typeof(LayoutDocumentGroupControl), new FrameworkPropertyMetadata(false));
        }

        internal LayoutDocumentGroupControl(ILayoutGroup model) : base(model)
        {
        }

        public override DragMode Mode
        {
            get
            {
                return DragMode.Document;
            }
        }

        public override void OnDrop(DragItem source)
        {
            if (DropMode == DropMode.Header
                || DropMode == DropMode.Center)
                base.OnDrop(source);
            else
            {
                var child = (source.RelativeObj as BaseFloatWindow).Child;
                (source.RelativeObj as BaseFloatWindow).DetachChild(child);

                if (_AssertSplitMode(DropMode))
                {
                    //must to changside
                    DockManager.ChangeSide(child, Model.Side);
                    if (DockViewParent == null)
                    {
                        var parent = Parent as ILayoutViewParent;
                        parent.DetachChild(this, false);
                        var panel = new LayoutGroupDocumentPanel()
                        {
                            Direction = (DropMode == DropMode.Left_WithSplit || DropMode == DropMode.Right_WithSplit) ? Direction.LeftToRight : Direction.UpToDown
                        };
                        panel._AttachChild(this, 0);
                        switch (DropMode)
                        {
                            case DropMode.Left_WithSplit:
                                panel.AttachChild(child, AttachMode.Left_WithSplit, 0);
                                break;
                            case DropMode.Top_WithSplit:
                                panel.AttachChild(child, AttachMode.Top_WithSplit, 0);
                                break;
                            case DropMode.Right_WithSplit:
                                panel.AttachChild(child, AttachMode.Right_WithSplit, 1);
                                break;
                            case DropMode.Bottom_WithSplit:
                                panel.AttachChild(child, AttachMode.Bottom_WithSplit, 1);
                                break;
                        }
                        parent.AttachChild(panel, AttachMode.None, 0);
                    }
                    else
                    {
                        var parent = Parent as LayoutGroupDocumentPanel;
                        parent.Direction = (DropMode == DropMode.Left_WithSplit || DropMode == DropMode.Right_WithSplit) ? Direction.LeftToRight : Direction.UpToDown;
                        int index = parent.IndexOf(this);
                        switch (DropMode)
                        {
                            case DropMode.Left_WithSplit:
                                parent.AttachChild(child, AttachMode.Left_WithSplit, index);
                                break;
                            case DropMode.Top_WithSplit:
                                parent.AttachChild(child, AttachMode.Top_WithSplit, index);
                                break;
                            case DropMode.Right_WithSplit:
                                parent.AttachChild(child, AttachMode.Right_WithSplit, index + 1);
                                break;
                            case DropMode.Bottom_WithSplit:
                                parent.AttachChild(child, AttachMode.Bottom_WithSplit, index + 1);
                                break;
                        }
                    }
                }
                else
                {
                    var _parent = Parent as LayoutGroupDocumentPanel;
                    if (_parent.DockViewParent is LayoutRootPanel)
                    {
                        var rootPanel = _parent.DockViewParent as LayoutRootPanel;
                        rootPanel.DetachChild(_parent, false);
                        var pparent = new LayoutGroupPanel()
                        {
                            Direction = (DropMode == DropMode.Left || DropMode == DropMode.Right) ? Direction.LeftToRight : Direction.UpToDown
                        };
                        pparent._AttachChild(_parent, 0);
                        switch (DropMode)
                        {
                            case DropMode.Left:
                                DockManager.ChangeSide(child, DockSide.Left);
                                pparent.AttachChild(child, AttachMode.Left, 0);
                                break;
                            case DropMode.Top:
                                DockManager.ChangeSide(child, DockSide.Top);
                                pparent.AttachChild(child, AttachMode.Top, 0);
                                break;
                            case DropMode.Right:
                                DockManager.ChangeSide(child, DockSide.Right);
                                pparent.AttachChild(child, AttachMode.Right, 1);
                                break;
                            case DropMode.Bottom:
                                DockManager.ChangeSide(child, DockSide.Bottom);
                                pparent.AttachChild(child, AttachMode.Bottom, 1);
                                break;
                        }
                        rootPanel.AttachChild(pparent, AttachMode.None, 0);
                    }
                    else
                    {
                        var panel = _parent.DockViewParent as LayoutGroupPanel;
                        int index = panel.IndexOf(_parent);
                        switch (DropMode)
                        {
                            case DropMode.Left:
                                DockManager.ChangeSide(child, DockSide.Left);
                                if (panel.Direction == Direction.LeftToRight)
                                    panel.AttachChild(child, AttachMode.Left, index);
                                else
                                {
                                    panel._DetachChild(_parent);
                                    var pparent = new LayoutGroupPanel()
                                    {
                                        Direction = Direction.LeftToRight
                                    };
                                    pparent._AttachChild(_parent, 0);
                                    pparent._AttachChild(child, 0);
                                    panel._AttachChild(pparent, Math.Min(index, panel.Count));
                                }
                                break;
                            case DropMode.Top:
                                DockManager.ChangeSide(child, DockSide.Top);
                                if (panel.Direction == Direction.UpToDown)
                                    panel.AttachChild(child, AttachMode.Top, index);
                                else
                                {
                                    panel._DetachChild(_parent);
                                    var pparent = new LayoutGroupPanel()
                                    {
                                        Direction = Direction.UpToDown
                                    };
                                    pparent._AttachChild(_parent, 0);
                                    pparent._AttachChild(child, 0);
                                    panel._AttachChild(pparent, Math.Min(index, panel.Count));
                                }
                                break;
                            case DropMode.Right:
                                DockManager.ChangeSide(child, DockSide.Right);
                                if (panel.Direction == Direction.LeftToRight)
                                    panel.AttachChild(child, AttachMode.Right, index + 1);
                                else
                                {
                                    panel._DetachChild(_parent);
                                    var pparent = new LayoutGroupPanel()
                                    {
                                        Direction = Direction.LeftToRight
                                    };
                                    pparent._AttachChild(_parent, 0);
                                    pparent._AttachChild(child, 1);
                                    panel._AttachChild(pparent, Math.Min(index, panel.Count));
                                }
                                break;
                            case DropMode.Bottom:
                                DockManager.ChangeSide(child, DockSide.Bottom);
                                if (panel.Direction == Direction.UpToDown)
                                    panel.AttachChild(child, AttachMode.Bottom, index + 1);
                                else
                                {
                                    panel._DetachChild(_parent);
                                    var pparent = new LayoutGroupPanel()
                                    {
                                        Direction = Direction.UpToDown
                                    };
                                    pparent._AttachChild(_parent, 0);
                                    pparent._AttachChild(child, 1);
                                    panel._AttachChild(pparent, Math.Min(index, panel.Count));
                                }
                                break;
                        }
                    }
                }
            }
            (source.RelativeObj as BaseFloatWindow).Close();
        }

        private bool _AssertSplitMode(DropMode mode)
        {
            return DropMode == DropMode.Left_WithSplit
                || DropMode == DropMode.Right_WithSplit
                || DropMode == DropMode.Top_WithSplit
                || DropMode == DropMode.Bottom_WithSplit;
        }
    }
}