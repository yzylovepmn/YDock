using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class AnchorHeaderControl : Control, IDisposable
    {
        static AnchorHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorHeaderControl), new FrameworkPropertyMetadata(default(AnchorHeaderControl)));
        }

        public void Dispose()
        {
            DataContext = null;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var wnd = Window.GetWindow(this);
                if (wnd is SingleAnchorWindow)
                {
                    if (wnd.WindowState == WindowState.Normal)
                        wnd.WindowState = WindowState.Maximized;
                    else wnd.WindowState = WindowState.Normal;
                }
            }
        }

        Point _mouseDown;
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _mouseDown = e.GetPosition(this);
            if(!IsMouseCaptured)
                CaptureMouse();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
            {
                if ((e.GetPosition(this) - _mouseDown).Length > Math.Max(SystemParameters.MinimumHorizontalDragDistance, SystemParameters.MinimumVerticalDragDistance))
                {
                    ReleaseMouseCapture();
                    IDockElement ele = DataContext as IDockElement;
                    if (!ele.DockManager.DragManager.IsDragging)
                    {
                        if (ele.Mode == DockMode.DockBar)
                            ele.DockManager.DragManager.IntoDragAction(new DragItem(ele, ele.Mode, _mouseDown, Rect.Empty));
                        else ele.DockManager.DragManager.IntoDragAction(new DragItem(ele.Container, ele.Mode, _mouseDown, Rect.Empty));
                    }
                }
            }
        }
    }
}