using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using YDock.Commands;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public abstract class BaseFloatWindow : Window, ILayoutViewParent
    {
        protected BaseFloatWindow(DockManager dockManager, bool needReCreate = false)
        {
            _dockManager = dockManager;
            MinWidth = 150;
            MinHeight = 60;
            _widthEceeed = Constants.FloatWindowResizeLength * 2;
            _heightEceeed = Constants.FloatWindowResizeLength * 2;
            NeedReCreate = needReCreate;
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;
            ShowActivated = true;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (Mouse.LeftButton == MouseButtonState.Pressed && (DockManager.DragManager._dragWnd == null || DockManager.DragManager._dragWnd == this))
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
                    {
                        _isDragging = true;
                        if (this is AnchorGroupWindow)
                            DockManager.DragManager.IntoDragAction(new DragItem(this, DockMode.Float, DragMode.Anchor, new Point(), Rect.Empty, new Size(ActualWidth, ActualWidth)), true);
                        else DockManager.DragManager.IntoDragAction(new DragItem(this, DockMode.Float, DragMode.Document, new Point(), Rect.Empty, new Size(ActualWidth, ActualWidth)), true);
                    }
                    break;
                case Win32Helper.WM_MOVING:
                    if (DockManager.DragManager.IsDragging)
                        DockManager.DragManager.OnMouseMove();
                    else
                    {
                        _isDragging = true;
                        if (this is AnchorGroupWindow)
                            DockManager.DragManager.IntoDragAction(new DragItem(this, DockMode.Float, DragMode.Anchor, new Point(), Rect.Empty, new Size(ActualWidth, ActualWidth)), true);
                        else DockManager.DragManager.IntoDragAction(new DragItem(this, DockMode.Float, DragMode.Document, new Point(), Rect.Empty, new Size(ActualWidth, ActualWidth)), true);
                    }
                    break;
                case Win32Helper.WM_EXITSIZEMOVE:
                    if (DockManager.DragManager.IsDragging)
                    {
                        DockManager.DragManager.DoDragDrop();
                        _isDragging = false;
                    }
                    _UpdateLocation(Child);
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

        private void _UpdateLocation(object obj)
        {
            if (obj != null)
            {
                if (obj is LayoutGroupPanel)
                    foreach (var child in (obj as LayoutGroupPanel).Children)
                        _UpdateLocation(child as IDockView);

                if (obj is BaseGroupControl)
                    _UpdateLocation((obj as BaseGroupControl).Model);

                if (obj is BaseLayoutGroup)
                {
                    foreach (DockElement item in (obj as BaseLayoutGroup).Children)
                    {
                        item.FloatLeft = Left;
                        item.FloatTop = Top;
                    }
                }
            }
        }

        #region Command
        protected override void OnInitialized(EventArgs e)
        {
            CommandBindings.Add(new CommandBinding(GlobalCommands.CloseCommand, OnCloseExecute, OnCloseCanExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.RestoreCommand, OnRestoreExecute, OnRestoreCanExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.MaximizeCommand, OnMaximizeExecute, OnMaximizeCanExecute));
            base.OnInitialized(e);
        }

        protected virtual void OnMaximizeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        protected void OnMaximizeExecute(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        protected virtual void OnRestoreCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        protected void OnRestoreExecute(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        protected void OnCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        protected void OnCloseExecute(object sender, ExecutedRoutedEventArgs e)
        {
            var child = Child;
            DetachChild(Child);
            if (child is IDisposable)
                (child as IDisposable).Dispose();
            Close();
        }
        #endregion

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

        internal Rect Location
        {
            get
            {
                return new Rect(Left, Top, Width, Height);
            }
        }

        protected bool _isDragging = false;
        public bool IsDragging
        {
            get { return _isDragging; }
        }

        protected DockManager _dockManager;

        public virtual DockManager DockManager
        {
            get
            {
                return _dockManager;
            }
            internal set
            {
                _dockManager = value;
            }
        }

        public virtual void Recreate() { }

        public void HitTest(Point p)
        {
            var p1 = (Content as FrameworkElement).PointToScreenDPIWithoutFlowDirection(new Point());
            VisualTreeHelper.HitTest(Content as FrameworkElement, _HitFilter, _HitRessult, new PointHitTestParameters(new Point(p.X - p1.X, p.Y - p1.Y)));
        }

        private HitTestResultBehavior _HitRessult(HitTestResult result)
        {
            DockManager.DragManager.DragTarget = null;
            return HitTestResultBehavior.Stop;
        }

        private HitTestFilterBehavior _HitFilter(DependencyObject potentialHitTestTarget)
        {
            if (potentialHitTestTarget is BaseGroupControl)
            {
                //设置DragTarget，以实时显示TargetWnd
                DockManager.DragManager.DragTarget = potentialHitTestTarget as IDragTarget;
                return HitTestFilterBehavior.Stop;
            }
            return HitTestFilterBehavior.Continue;
        }

        public virtual void DetachChild(IDockView child, bool force = true)
        {
            if (child == Content)
            {
                DockManager.RemoveFloatWindow(this);
                if (child is BaseGroupControl)
                    (child as BaseGroupControl).IsDraggingFromDock = false;
                Content = null;
                if (force)
                    _dockManager = null;
            }
        }

        public virtual void AttachChild(IDockView child, AttachMode mode, int index)
        {
            if (Content != child)
            {
                Content = child;
                DockManager.AddFloatWindow(this);
                Height = (child as ILayoutSize).DesiredHeight + _heightEceeed;
                Width = (child as ILayoutSize).DesiredWidth + _widthEceeed;
            }
        }

        public int IndexOf(IDockView child)
        {
            if (child == Child)
                return 0;
            else return -1;
        }
    }
}