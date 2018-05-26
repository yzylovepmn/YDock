using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;
using YDock.View;

namespace YDock
{
    public class DragItem : IDisposable
    {
        internal DragItem(object relativeObj, DockMode dockMode, DragMode dragMode, Point clickPos, Rect clickRect, Size size)
        {
            _relativeObj = relativeObj;
            _dockMode = dockMode;
            _dragMode = dragMode;
            _clickPos = clickPos;
            _clickRect = clickRect;
            _size = size;
        }

        private object _relativeObj;
        public object RelativeObj
        {
            get { return _relativeObj; }
        }
        /// <summary>
        /// 拖动前的Mode
        /// </summary>
        public DockMode DockMode
        {
            get { return _dockMode; }
        }
        private DockMode _dockMode;

        public DragMode DragMode
        {
            get { return _dragMode; }
        }
        private DragMode _dragMode;

        public Point ClickPos
        {
            get { return _clickPos; }
        }
        private Point _clickPos;

        public Rect ClickRect
        {
            get { return _clickRect; }
        }
        private Rect _clickRect;

        private Size _size;
        public Size Size
        {
            get { return _size; }
        }

        public void Dispose()
        {
            _relativeObj = null;
        }
    }

    public class DragManager
    {
        internal DragManager(DockManager dockManager)
        {
            _dockManager = dockManager;
        }

        #region DockManager
        private DockManager _dockManager;
        public DockManager DockManager
        {
            get { return _dockManager; }
        }
        #endregion

        #region Drag
        #region private field
        private Point _mouseP;
        private Size _rootSize;
        private IDragTarget _dragTarget;
        private DragItem _dragItem;
        internal BaseFloatWindow _dragWnd;
        private bool _isDragging = false;
        #endregion

        #region Property
        internal DragItem DragItem
        {
            get { return _dragItem; }
            set
            {
                if (_dragItem != value)
                    _dragItem = value;
            }
        }

        internal IDragTarget DragTarget
        {
            get { return _dragTarget; }
            set
            {
                if (_dragTarget != value)
                {
                    if (_dragTarget != null)
                        _dragTarget.HideDropWindow();
                    _dragTarget = value;
                    if (_dragTarget != null)
                        _dragTarget.ShowDropWindow();
                }
                else if (_dragTarget != null && !_isDragOverRoot)
                {
                    if (_dragItem.DragMode == DragMode.Document && _dragTarget.Mode == DragMode.Anchor)
                        return;
                    _dragTarget.Update(_mouseP);
                }
            }
        }

        public bool IsDragging
        {
            get { return _isDragging; }
            set
            {
                if (_isDragging != value)
                {
                    _isDragging = value;
                    OnDragStatusChanged(new DragStatusChangedEventArgs(value));
                }
            }
        }

        private bool _isDragOverRoot = false;
        public bool IsDragOverRoot
        {
            get { return _isDragOverRoot; }
            set { _isDragOverRoot = value; }
        }

        public event DragStatusChanged OnDragStatusChanged = delegate { };
        #endregion

        #region Drag Action
        internal void IntoDragAction(DragItem dragItem, bool _isInvokeByFloatWnd = false)
        {
            DockManager.UpdateWindowZOrder();

            _dragItem = dragItem;
            //被浮动窗口调用则不需要调用BeforeDrag()
            if (_isInvokeByFloatWnd)
            {
                IsDragging = true;
                if (_dragWnd == null)
                    _dragWnd = _dragItem.RelativeObj as BaseFloatWindow;
            }
            else BeforeDrag();

            //初始化最外层的_rootTarget
            _rootSize = DockManager.LayoutRootPanel.RootGroupPanel.TransformActualSizeToAncestor();

            if (!_isInvokeByFloatWnd && _dragWnd is DocumentGroupWindow)
                _dragWnd.Recreate();
        }

        private void BeforeDrag()
        {
            _InitDragItem();
        }

        internal void DoDragDrop()
        {
            if (DockManager.LayoutRootPanel.RootGroupPanel?.DropMode != DropMode.None)
                DockManager.LayoutRootPanel.RootGroupPanel?.OnDrop(_dragItem);
            else if (DragTarget?.DropMode != DropMode.None)
                DragTarget?.OnDrop(_dragItem);

            IsDragging = false;

            AfterDrag();
        }

        private void AfterDrag()
        {
            if (_dragWnd != null && _dragWnd.NeedReCreate)
                _dragWnd.Recreate();
            _dragWnd = null;
            DockManager.LayoutRootPanel.RootGroupPanel.CloseDropWindow();
            _DestroyDragItem();

            _isDragOverRoot = false;
            BaseDropPanel.ActiveVisual = null;
            BaseDropPanel.CurrentRect = null;
            CommandManager.InvalidateRequerySuggested();
        }
        #endregion

        #region init & destroy
        private void _InitDragItem()
        {
            LayoutGroup group;
            IDockElement ele;
            var mouseP = DockHelper.GetMousePosition(DockManager);
            switch (_dragItem.DockMode)
            {
                case DockMode.Normal:
                    if (_dragItem.RelativeObj is ILayoutGroup)
                    {
                        var _layoutGroup = _dragItem.RelativeObj as LayoutGroup;

                        #region AttachObj
                        var _parent = _layoutGroup.View.DockViewParent as LayoutGroupPanel;
                        var _mode = _parent.Direction == Direction.LeftToRight ? AttachMode.Left : AttachMode.Top;
                        if (_parent.Direction == Direction.None)
                                _mode = AttachMode.None;
                        var _index = _parent.IndexOf(_layoutGroup.View);
                        if (_parent.Children.Count - 1 > _index)
                            _layoutGroup.AttachObj = new AttachObject(_layoutGroup, _parent.Children[_index + 2] as INotifyDisposable, _index, _mode);
                        else _layoutGroup.AttachObj = new AttachObject(_layoutGroup, _parent.Children[_index - 2] as INotifyDisposable, _index, _mode);
                        #endregion

                        //这里移动的一定是AnchorSideGroup，故将其从父级LayoutGroupPanel移走，但不Dispose留着构造浮动窗口
                        if ((_layoutGroup.View as ILayoutGroupControl).TryDeatchFromParent(false))
                        {
                            //注意重新设置Mode
                            (_layoutGroup as BaseLayoutGroup).Mode = DockMode.Float;
                            _dragWnd = new AnchorGroupWindow(DockManager)
                            {
                                Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - 1,
                                Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - 1
                            };
                            _dragWnd.AttachChild(_layoutGroup.View, AttachMode.None, 0);
                            _dragWnd.Show();
                        }
                    }
                    else if (_dragItem.RelativeObj is IDockElement)
                    {
                        ele = _dragItem.RelativeObj as IDockElement;

                        #region AttachObj
                        var _parent = (ele.Container as LayoutGroup).View as BaseGroupControl;
                        var _index = ele.Container.IndexOf(ele);
                        #endregion

                        if (ele.IsDocument)
                            group = new LayoutDocumentGroup(DockMode.Float, DockManager);
                        else
                        {
                            group = new LayoutGroup(ele.Side, DockMode.Float, DockManager);
                            group.AttachObj = new AttachObject(group, _parent, _index);
                        }
                        //先从逻辑父级中移除
                        ele.Container.Detach(ele);
                        //再加入新的逻辑父级
                        group.Attach(ele);
                        //创建新的浮动窗口，并初始化位置
                        if (ele.IsDocument)
                        {
                            _dragWnd = new DocumentGroupWindow(DockManager) { NeedReCreate = true };
                            _dragWnd.AttachChild(new LayoutDocumentGroupControl(group) { IsDraggingFromDock = true, BorderThickness = new Thickness(1) }, AttachMode.None, 0);
                            _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                            _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                        }
                        else
                        {
                            _dragWnd = new AnchorGroupWindow(DockManager) { NeedReCreate = _dragItem.DragMode == DragMode.Anchor };
                            _dragWnd.AttachChild(new AnchorSideGroupControl(group) { IsDraggingFromDock = _dragItem.DragMode == DragMode.Anchor }, AttachMode.None, 0);
                            if (!_dragWnd.NeedReCreate)
                            {
                                _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength;
                                _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            }
                            else
                            {
                                _dragWnd.Top = mouseP.Y + _dragItem.ClickPos.Y + Constants.FloatWindowResizeLength - _dragWnd.Height;
                                _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            }
                        }
                        if (_dragWnd.NeedReCreate)
                        {
                            _dragWnd.Background = Brushes.Transparent;
                            _dragWnd.BorderBrush = Brushes.Transparent;
                        }
                        _dragWnd.Show();
                    }
                    break;
                case DockMode.DockBar:
                    //这里表示从自动隐藏窗口进行的拖动，因此这里移除自动隐藏窗口
                    ele = _dragItem.RelativeObj as IDockElement;
                    ele.Container.Detach(ele);
                    //创建新的浮动窗口，并初始化位置
                    group = new LayoutGroup(ele.Side, DockMode.Float, DockManager);
                    group.Attach(ele);
                    _dragWnd = new AnchorGroupWindow(DockManager)
                    {
                        Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - 1,
                        Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - 1
                    };
                    _dragWnd.AttachChild(new AnchorSideGroupControl(group), AttachMode.None, 0);
                    _dragWnd.Show();
                    break;
                case DockMode.Float:
                    if (_dragItem.RelativeObj is IDockElement)
                    {
                        ele = _dragItem.RelativeObj as IDockElement;
                        var ctrl = ele.Container.View as BaseGroupControl;
                        if (ctrl.Items.Count == 1 && ctrl.Parent is BaseFloatWindow)
                        {
                            _dragWnd = ctrl.Parent as BaseFloatWindow;
                            _dragWnd.DetachChild(ctrl);
                            _dragWnd.Close();
                            ctrl.BorderThickness = new Thickness(1);
                            ctrl.IsDraggingFromDock = true;
                            _dragWnd = new DocumentGroupWindow(DockManager) { NeedReCreate = true, Background = Brushes.Transparent, BorderBrush = Brushes.Transparent };
                            _dragWnd.AttachChild(ctrl, AttachMode.None, 0);
                            _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                            _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            _dragWnd.Show();
                        }
                        else
                        {
                            #region AttachObj
                            var _parent = (ele.Container as LayoutGroup).View as BaseGroupControl;
                            var _index = ele.Container.IndexOf(ele);
                            #endregion

                            if (ele.IsDocument)
                                group = new LayoutDocumentGroup(DockMode.Float, DockManager);
                            else
                            {
                                group = new LayoutGroup(ele.Side, DockMode.Float, DockManager);
                                group.AttachObj = new AttachObject(group, _parent, _index);
                            }
                            //先从逻辑父级中移除
                            ele.Container.Detach(ele);
                            //再加入新的逻辑父级
                            group.Attach(ele);
                            //创建新的浮动窗口，并初始化位置
                            //这里可知引起drag的时DragTabItem故这里创建临时的DragTabWindow
                            if (ele.IsDocument)
                            {
                                _dragWnd = new DocumentGroupWindow(DockManager) { NeedReCreate = true };
                                _dragWnd.AttachChild(new LayoutDocumentGroupControl(group) { IsDraggingFromDock = true, BorderThickness = new Thickness(1) }, AttachMode.None, 0);
                                _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                                _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            }
                            else
                            {
                                _dragWnd = new AnchorGroupWindow(DockManager) { NeedReCreate = _dragItem.DragMode == DragMode.Anchor };
                                _dragWnd.AttachChild(new AnchorSideGroupControl(group) { IsDraggingFromDock = _dragItem.DragMode == DragMode.Anchor }, AttachMode.None, 0);
                                if (!_dragWnd.NeedReCreate)
                                {
                                    _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength;
                                    _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                                }
                                else
                                {
                                    _dragWnd.Top = mouseP.Y + _dragItem.ClickPos.Y + Constants.FloatWindowResizeLength - _dragWnd.Height;
                                    _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                                }
                            }
                            if (_dragWnd.NeedReCreate)
                            {
                                _dragWnd.Background = Brushes.Transparent;
                                _dragWnd.BorderBrush = Brushes.Transparent;
                            }
                            _dragWnd.Show();
                        }
                    }
                    else if (_dragItem.RelativeObj is ILayoutGroup)
                    {
                        group = _dragItem.RelativeObj as LayoutGroup;
                        //表示此时的浮动窗口为IsSingleMode
                        if (group.View.DockViewParent == null)
                            _dragWnd = (group.View as BaseGroupControl).Parent as BaseFloatWindow;
                        else
                        {
                            #region AttachObj
                            var _parent = group.View.DockViewParent as LayoutGroupPanel;
                            var _mode = _parent.Direction == Direction.LeftToRight ? AttachMode.Left : AttachMode.Top;
                            if (_parent.Direction == Direction.None)
                                _mode = AttachMode.None;
                            var _index = _parent.IndexOf(group.View);
                            if (_parent.Children.Count - 1 > _index)
                                group.AttachObj = new AttachObject(group, _parent.Children[_index + 2] as INotifyDisposable, _index, _mode);
                            else group.AttachObj = new AttachObject(group, _parent.Children[_index - 2] as INotifyDisposable, _index, _mode);
                            #endregion

                            //这里移动的一定是AnchorSideGroup，故将其从父级LayoutGroupPanel移走，但不Dispose留着构造浮动窗口
                            if ((group.View as ILayoutGroupControl).TryDeatchFromParent(false))
                            {
                                _dragWnd = new AnchorGroupWindow(DockManager)
                                {
                                    Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - 1,
                                    Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - 1
                                };
                                _dragWnd.AttachChild(group.View, AttachMode.None, 0);
                                _dragWnd.Show();
                            }
                        }
                    }
                    break;
            }
        }

        private void _DestroyDragItem()
        {
            _dragItem.Dispose();
            _dragItem = null;
            DragTarget = null;
        }
        #endregion

        #endregion

        #region Flag
        internal const int NONE = 0x0000;
        internal const int LEFT = 0x0001;
        internal const int TOP = 0x0002;
        internal const int RIGHT = 0x0004;
        internal const int BOTTOM = 0x0008;
        internal const int CENTER = 0x0010;
        internal const int TAB = 0x0020;
        internal const int HEAD = 0x0040;
        internal const int SPLIT = 0x1000;
        internal const int ACTIVE = 0x2000;
        #endregion

        #region DragEvent
        internal void OnMouseMove()
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                DockManager.LayoutRootPanel.RootGroupPanel?.HideDropWindow();
                DragTarget = null;
                return;
            }
            bool flag = false;
            _mouseP = DockHelper.GetMousePosition(DockManager);
            foreach (var wnd in DockManager.FloatWindows)
            {
                if (wnd != _dragWnd
                    && wnd.Location.Contains(_mouseP)
                    && !(wnd is DocumentGroupWindow && _dragItem.DragMode == DragMode.Anchor)
                    && !(wnd is AnchorGroupWindow && _dragItem.DragMode == DragMode.Document))
                {
                    if (wnd is DocumentGroupWindow)
                        if (DockManager.IsBehindToMainWindow(wnd))
                            continue;

                    if (wnd != DockManager.FloatWindows.First())
                    {
                        DockManager.MoveFloatTo(wnd);
                        wnd.Activate();
                        _dragWnd.Activate();
                    }
                    wnd.HitTest(_mouseP);

                    DockManager.LayoutRootPanel.RootGroupPanel.HideDropWindow();
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                var p = DockHelper.GetMousePositionRelativeTo(DockManager.LayoutRootPanel.RootGroupPanel);
                if (p.X >= 0 && p.Y >= 0
                    && p.X <= _rootSize.Width
                    && p.Y <= _rootSize.Height)
                {
                    if (_dragItem.DragMode != DragMode.Document)
                    {
                        DockManager.LayoutRootPanel.RootGroupPanel?.ShowDropWindow();
                        DockManager.LayoutRootPanel.RootGroupPanel?.Update(_mouseP);
                    }
                    VisualTreeHelper.HitTest(DockManager.LayoutRootPanel.RootGroupPanel, _HitFilter, _HitRessult, new PointHitTestParameters(p));
                }
                else
                {
                    if (_dragItem.DragMode != DragMode.Document)
                        DockManager.LayoutRootPanel.RootGroupPanel?.HideDropWindow();
                    DragTarget = null;
                }
            }
        }

        private HitTestResultBehavior _HitRessult(HitTestResult result)
        {
            DragTarget = null;
            return HitTestResultBehavior.Stop;
        }

        private HitTestFilterBehavior _HitFilter(DependencyObject potentialHitTestTarget)
        {
            if (potentialHitTestTarget is BaseGroupControl)
            {
                //设置DragTarget，以实时显示TargetWnd
                DragTarget = potentialHitTestTarget as IDragTarget;
                return HitTestFilterBehavior.Stop;
            }
            return HitTestFilterBehavior.Continue;
        }
        #endregion
    }

    public class DragStatusChangedEventArgs : EventArgs
    {
        internal DragStatusChangedEventArgs(bool status)
        {
            _isDragging = status;
        }

        private bool _isDragging;
        public bool IsDragging
        {
            get { return _isDragging; }
        }
    }

    public delegate void DragStatusChanged(DragStatusChangedEventArgs args);
}