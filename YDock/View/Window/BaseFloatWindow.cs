using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using YDock.Enum;
using YDock.Interface;

namespace YDock.View
{
    public abstract class BaseFloatWindow : Window, ILayoutViewParent
    {
        protected BaseFloatWindow(bool needReCreate = false)
        {
            MinWidth = 150;
            MinHeight = 60;
            _widthEceeed = Constants.FloatWindowResizeLength * 2;
            _heightEceeed = Constants.FloatWindowResizeLength * 2;
            _needReCreate = needReCreate;
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;
            ShowActivated = true;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                var mousePosition = this.PointToScreenDPI(Mouse.GetPosition(this));
                IntPtr lParam = new IntPtr(((int)mousePosition.X & (int)0xFFFF) | (((int)mousePosition.Y) << 16));

                Win32Helper.SendMessage(windowHandle, Win32Helper.WM_NCLBUTTONDOWN, new IntPtr(Win32Helper.HT_CAPTION), lParam);
            }
        }

        protected HwndSource _hwndSrc;
        protected HwndSourceHook _hwndSrcHook;

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(OnLoaded);

            _hwndSrc = PresentationSource.FromDependencyObject(this) as HwndSource;
            _hwndSrcHook = new HwndSourceHook(FilterMessage);
            _hwndSrc.AddHook(_hwndSrcHook);
        }

        protected virtual IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            switch (msg)
            {
                case Win32Helper.WM_ENTERSIZEMOVE:
                    if (!DockManager.DragManager.IsDragging)
                        DockManager.DragManager.IntoDragAction(new DragItem(this, DockMode.Float, new Point(), Rect.Empty));
                    break;
                case Win32Helper.WM_MOVING:
                    if (DockManager.DragManager.IsDragging)
                        DockManager.DragManager.OnMouseMove(this);
                    break;
                case Win32Helper.WM_EXITSIZEMOVE:
                    if (DockManager.DragManager.IsDragging)
                        DockManager.DragManager.DoDragDrop();
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= new RoutedEventHandler(OnUnloaded);

            if (_hwndSrc != null)
            {
                _hwndSrc.RemoveHook(_hwndSrcHook);
                _hwndSrc.Dispose();
                _hwndSrc = null;
            }
        }

        protected double _widthEceeed;
        internal double WidthEceeed
        {
            get { return _widthEceeed; }
        }

        protected double _heightEceeed;
        internal double HeightEceeed
        {
            get { return _heightEceeed; }
        }

        internal virtual ILayoutViewWithSize Child
        {
            get
            {
                return Content == null ? null : Content as ILayoutViewWithSize;
            }
        }

        protected bool _needReCreate;
        internal bool NeedReCreate
        {
            get { return _needReCreate; }
            set { _needReCreate = value; }
        }

        public virtual DockManager DockManager
        {
            get
            {
                if (Content != null)
                {
                    if (Content is ILayoutPanel)
                        return (Content as LayoutGroupPanel).DockManager;
                    else return Child.Model.DockManager;
                }
                return null;
            }
        }

        public virtual void AttachChild(IDockView child, int index)
        {
            if (Content != child)
            {
                Content = child;
                Height = (child as ILayoutSize).DesiredHeight + _heightEceeed;
                Width = (child as ILayoutSize).DesiredWidth + _widthEceeed;
            }
        }

        public virtual void DetachChild(IDockView child)
        {
            if (child == Content)
                Content = null;
        }

        public virtual void Recreate() { }
    }
}