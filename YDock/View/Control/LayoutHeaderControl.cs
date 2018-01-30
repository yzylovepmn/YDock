using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YDock.Interface;

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
            _mouseDown = e.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if ((e.GetPosition(this) - _mouseDown).Length > Math.Max(SystemParameters.MinimumHorizontalDragDistance, SystemParameters.MinimumVerticalDragDistance))
                {
                    IDockElement ele = DataContext as IDockElement;
                    if (!ele.DockManager.DragManager.IsDragging)
                        ele.DockManager.DragManager.IntoDragAction(ele.Container);
                }
            }
        }
    }
}