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

        internal LayoutDocumentGroupControl(ILayoutGroup model, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength) : base(model, desiredWidth, desiredHeight)
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
                IDockView child;
                if (source.RelativeObj is BaseFloatWindow)
                {
                    child = (source.RelativeObj as BaseFloatWindow).Child;
                    (source.RelativeObj as BaseFloatWindow).DetachChild(child);
                }
                else child = source.RelativeObj as IDockView;
                DockManager.ChangeDockMode(child, (Model as ILayoutGroup).Mode);

                if (_AssertSplitMode(DropMode))
                {
                    //must to changside
                    DockManager.ChangeSide(child, Model.Side);
                    if (DockViewParent == null)
                    {
                        var parent = Parent as BaseFloatWindow;
                        parent.DetachChild(this, false);
                        var panel = new LayoutGroupDocumentPanel()
                        {
                            DesiredWidth = Math.Max(parent.ActualWidth, Constants.DockDefaultWidthLength),
                            DesiredHeight = Math.Max(parent.ActualHeight, Constants.DockDefaultHeightLength),
                            Direction = (DropMode == DropMode.Left_WithSplit || DropMode == DropMode.Right_WithSplit) ? Direction.Horizontal : Direction.Vertical
                        };
                        panel._AttachChild(this, 0);
                        if (DropMode == DropMode.Left_WithSplit || DropMode == DropMode.Top_WithSplit)
                            panel.AttachChild(child, DropMode == DropMode.Left_WithSplit ? AttachMode.Left_WithSplit : AttachMode.Top_WithSplit, 0);
                        else panel.AttachChild(child, DropMode == DropMode.Right_WithSplit ? AttachMode.Right_WithSplit : AttachMode.Bottom_WithSplit, 1);
                        parent.AttachChild(panel, AttachMode.None, 0);
                    }
                    else
                    {
                        var parent = Parent as LayoutGroupDocumentPanel;
                        parent.Direction = (DropMode == DropMode.Left_WithSplit || DropMode == DropMode.Right_WithSplit) ? Direction.Horizontal : Direction.Vertical;
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
                    DockManager.FormatChildSize(child as ILayoutSize, new Size(ActualWidth, ActualHeight));

                    var _parent = Parent as LayoutGroupDocumentPanel;
                    var child_size = child as ILayoutSize;
                    if (_parent.DockViewParent is LayoutRootPanel)
                    {
                        var rootPanel = _parent.DockViewParent as LayoutRootPanel;
                        rootPanel.DetachChild(_parent, false);
                        var pparent = new LayoutGroupPanel()
                        {
                            Direction = (DropMode == DropMode.Left || DropMode == DropMode.Right) ? Direction.Horizontal : Direction.Vertical
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
                                if (panel.Direction == Direction.Horizontal)
                                    panel.AttachChild(child, AttachMode.Left, index);
                                else
                                {
                                    panel._DetachChild(_parent);
                                    var pparent = new LayoutGroupPanel()
                                    {
                                        Direction = Direction.Horizontal
                                    };
                                    pparent._AttachChild(_parent, 0);
                                    pparent._AttachChild(child, 0);
                                    panel._AttachChild(pparent, Math.Min(index, panel.Count));
                                }
                                break;
                            case DropMode.Top:
                                DockManager.ChangeSide(child, DockSide.Top);
                                if (panel.Direction == Direction.Vertical)
                                    panel.AttachChild(child, AttachMode.Top, index);
                                else
                                {
                                    panel._DetachChild(_parent);
                                    var pparent = new LayoutGroupPanel()
                                    {
                                        Direction = Direction.Vertical
                                    };
                                    pparent._AttachChild(_parent, 0);
                                    pparent._AttachChild(child, 0);
                                    panel._AttachChild(pparent, Math.Min(index, panel.Count));
                                }
                                break;
                            case DropMode.Right:
                                DockManager.ChangeSide(child, DockSide.Right);
                                if (panel.Direction == Direction.Horizontal)
                                    panel.AttachChild(child, AttachMode.Right, index + 1);
                                else
                                {
                                    panel._DetachChild(_parent);
                                    var pparent = new LayoutGroupPanel()
                                    {
                                        Direction = Direction.Horizontal
                                    };
                                    pparent._AttachChild(_parent, 0);
                                    pparent._AttachChild(child, 1);
                                    panel._AttachChild(pparent, Math.Min(index, panel.Count));
                                }
                                break;
                            case DropMode.Bottom:
                                DockManager.ChangeSide(child, DockSide.Bottom);
                                if (panel.Direction == Direction.Vertical)
                                    panel.AttachChild(child, AttachMode.Bottom, index + 1);
                                else
                                {
                                    panel._DetachChild(_parent);
                                    var pparent = new LayoutGroupPanel()
                                    {
                                        Direction = Direction.Vertical
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

            if (source.RelativeObj is BaseFloatWindow)
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