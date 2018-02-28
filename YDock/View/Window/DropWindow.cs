using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;

namespace YDock.View
{
    public class DropWindow : Popup, IDropWindow, IDisposable
    {
        public DropWindow(IDragTarget host)
        {
            AllowsTransparency = true;
            _host = host;
            if (host.Mode == DragMode.RootPanel)
            {
                _dropPanel = new RootDropPanel(host, host.DockManager.DragManager.DragItem);
                _dropPanel.SizeChanged += OnSizeChanged;
            }
            else _dropPanel = new DropPanel(host, host.DockManager.DragManager.DragItem);
            Child = _dropPanel;
            if (host.Mode != DragMode.RootPanel)
            {
                if (host.Mode == DragMode.Document
                    && host.DockManager.DragManager.DragItem.DragMode == DragMode.Anchor)
                {
                    MinWidth = Constants.DropUnitLength * 5;
                    MinHeight = Constants.DropUnitLength * 5;
                }
                else
                {
                    MinWidth = Constants.DropUnitLength * 3;
                    MinHeight = Constants.DropUnitLength * 3;
                }
            }
            else
            {
                MinWidth = 0;
                MinHeight = 0;
            }
        }

        //Popup在全屏时显示不全，这里将PopupRoot的高度强制为ScreenHeight
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _dropPanel.SizeChanged -= OnSizeChanged;
            DependencyObject parent = Child;
            do
            {
                parent = VisualTreeHelper.GetParent(parent);

                if (parent != null && parent.ToString() == "System.Windows.Controls.Primitives.PopupRoot")
                {
                    (parent as FrameworkElement).Height = (_host as FrameworkElement).ActualHeight;
                    break;
                }
            }
            while (parent != null);
        }

        private BaseDropPanel _dropPanel;
        public BaseDropPanel DropPanel
        {
            get { return _dropPanel; }
        }

        private IDragTarget _host;
        public IDragTarget Host
        {
            get { return _host; }
        }

        public void Hide()
        {
            Child.Visibility = Visibility.Hidden;
        }

        public void Show()
        {
            Child.Visibility = Visibility.Visible;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public void Update(Point mouseP)
        {
            _dropPanel.Update(mouseP);
        }

        public void Dispose()
        {
            _dropPanel.Dispose();
            _dropPanel = null;
            _host = null;
        }

        protected override void OnClosed(EventArgs e)
        {
            Dispose();
            base.OnClosed(e);
        }
    }
}