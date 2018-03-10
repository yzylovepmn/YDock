﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class AnchorSideGroupControl : BaseGroupControl
    {
        static AnchorSideGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorSideGroupControl), new FrameworkPropertyMetadata(typeof(AnchorSideGroupControl)));
            FocusableProperty.OverrideMetadata(typeof(AnchorSideGroupControl), new FrameworkPropertyMetadata(false));
        }

        internal AnchorSideGroupControl(ILayoutGroup model) : base(model)
        {
            
        }

        public override DragMode Mode
        {
            get
            {
                return DragMode.Anchor;
            }
        }

        public override void OnDrop(DragItem source)
        {
            if (DropMode == DropMode.Left
                || DropMode == DropMode.Right
                || DropMode == DropMode.Top
                || DropMode == DropMode.Bottom)
            {
                var child = (source.RelativeObj as BaseFloatWindow).Child;
                (source.RelativeObj as BaseFloatWindow).DetachChild(child);
                //must to changside
                DockManager.ChangeSide(child, Model.Side);

                LayoutGroupPanel panel;
                if (DockViewParent == null)
                {
                    var wnd = Parent as BaseFloatWindow;
                    wnd.DetachChild(this);
                    panel = new LayoutGroupPanel(Model.Side)
                    {
                        Direction = (DropMode == DropMode.Left || DropMode == DropMode.Right) ? Direction.LeftToRight : Direction.UpToDown,
                        DesiredWidth = wnd.ActualWidth,
                        DesiredHeight = wnd.ActualHeight,
                        IsAnchorPanel = true
                    };
                    wnd.DockManager = DockManager;
                    wnd.AttachChild(panel, AttachMode.None, 0);
                    panel._AttachChild(this, 0);
                }
                else panel = DockViewParent as LayoutGroupPanel;

                int index = panel.Children.IndexOf(this);
                switch (DropMode)
                {
                    case DropMode.Left:
                        if (panel.Direction == Direction.UpToDown)
                        {
                            var _subpanel = new LayoutGroupPanel(Model.Side)
                            {
                                Direction = Direction.LeftToRight,
                                DesiredWidth = ActualWidth,
                                DesiredHeight = ActualHeight,
                                IsAnchorPanel = true
                            };
                            panel._DetachChild(this);
                            panel._AttachChild(_subpanel, Math.Min(index, panel.Count));
                            _subpanel._AttachChild(this, 0);
                            _subpanel.AttachChild(child, AttachMode.Left, 0);
                        }
                        else panel._AttachChild(child, index);
                        break;
                    case DropMode.Top:
                        if (panel.Direction == Direction.LeftToRight)
                        {
                            var _subpanel = new LayoutGroupPanel(Model.Side)
                            {
                                Direction = Direction.UpToDown,
                                DesiredWidth = ActualWidth,
                                DesiredHeight = ActualHeight,
                                IsAnchorPanel = true
                            };
                            panel._DetachChild(this);
                            panel._AttachChild(_subpanel, Math.Min(index, panel.Count));
                            _subpanel._AttachChild(this, 0);
                            _subpanel.AttachChild(child, AttachMode.Top, 0);
                        }
                        else panel._AttachChild(child, index);
                        break;
                    case DropMode.Right:
                        if (panel.Direction == Direction.UpToDown)
                        {
                            var _subpanel = new LayoutGroupPanel(Model.Side)
                            {
                                Direction = Direction.LeftToRight,
                                DesiredWidth = ActualWidth,
                                DesiredHeight = ActualHeight,
                                IsAnchorPanel = true
                            };
                            panel._DetachChild(this);
                            _subpanel._AttachChild(this, 0);
                            _subpanel.AttachChild(child, AttachMode.Right, 1);
                            panel._AttachChild(_subpanel, Math.Min(index, panel.Count));
                        }
                        else panel._AttachChild(child, index + 1);
                        break;
                    case DropMode.Bottom:
                        if (panel.Direction == Direction.LeftToRight)
                        {
                            var _subpanel = new LayoutGroupPanel(Model.Side)
                            {
                                Direction = Direction.UpToDown,
                                DesiredWidth = ActualWidth,
                                DesiredHeight = ActualHeight,
                                IsAnchorPanel = true
                            };
                            panel._DetachChild(this);
                            _subpanel._AttachChild(this, 0);
                            _subpanel.AttachChild(child, AttachMode.Bottom, 1);
                            panel._AttachChild(_subpanel, Math.Min(index, panel.Count));
                        }
                        else panel._AttachChild(child, index + 1);
                        break;
                }
            }
            else base.OnDrop(source);
            (source.RelativeObj as BaseFloatWindow).Close();
        }
    }
}