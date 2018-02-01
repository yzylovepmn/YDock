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
    public class LayoutHeaderControl : Control, IDisposable
    {
        static LayoutHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutHeaderControl), new FrameworkPropertyMetadata(default(LayoutHeaderControl)));
        }

        public void Dispose()
        {
            DataContext = null;
        }

        Point _mouseDown;
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (IsMouseCaptured)
                ReleaseMouseCapture();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _mouseDown = e.GetPosition(this);
            if(!IsMouseCaptured)
                CaptureMouse();
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
                        ele.DockManager.DragManager.IntoDragAction(new DragItem(ele.Container, ele.Container is DockSideGroup ? DockMode.DockBar : DockMode.Normal, _mouseDown, new Rect()));
                }
            }
        }
    }
}