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
                Child = new RootDropPanel(host, host.DockManager.DragManager.DragItem);
                (Child as FrameworkElement).SizeChanged += OnSizeChanged;
            }
            else Child = new DropPanel(host, host.DockManager.DragManager.DragItem);
        }

        //Popup在全屏时显示不全，这里将PopupRoot的高度强制为ScreenHeight
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
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

        public void Update()
        {

        }

        public void Dispose()
        {
            (Child as BaseDropPanel).Dispose();
            Child = null;
            _host = null;
        }

        protected override void OnClosed(EventArgs e)
        {
            Dispose();
            base.OnClosed(e);
        }
    }
}