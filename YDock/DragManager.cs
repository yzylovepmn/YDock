using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;
using YDock.View;

namespace YDock
{
    public class DragItem : IDisposable
    {
        public DragItem(object relativeObj, DockMode mode, Point clickPos, Rect clickRect)
        {
            _relativeObj = relativeObj;
            _mode = mode;
            _clickPos = clickPos;
            _clickRect = clickRect;
        }

        private object _relativeObj;
        public object RelativeObj
        {
            get { return _relativeObj; }
        }
        /// <summary>
        /// 拖动前的Mode
        /// </summary>
        public DockMode Mode
        {
            get { return _mode; }
        }
        private DockMode _mode;

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
        private Rect _rootRect;
        private IDragTarget _rootTarget;
        private IDragTarget _dragTarget;
        internal IDragTarget DragTarget
        {
            get { return _dragTarget; }
            set
            {
                if (_dragTarget != value)
                {
                    if (_dragTarget != null)
                        _dragTarget.CloseDropWindow();
                    _dragTarget = value;
                    if (_dragTarget != null)
                    {
                        _dragTarget.CreateDropWindow();
                        _dragTarget.ShowDropWindow();
                    }
                }
            }
        }
        private DragItem _dragItem;
        private BaseFloatWindow _dragWnd;
        private bool _isDragging = false;
        #endregion

        #region Property
        public bool IsDragging
        {
            get { return _isDragging; }
        }
        #endregion

        #region Drag Action
        internal void IntoDragAction(DragItem dragItem)
        {
            _isDragging = true;
            _dragItem = dragItem;
            BeforeDrag();
        }

        private void BeforeDrag()
        {
            _rootRect = DockManager.LayoutRootPanel.RootGroupPanel.CreateRect();
            _rootTarget = DockManager.LayoutRootPanel.RootGroupPanel;
            _InitDragItem();
        }

        internal void DoDragDrop()
        {
            _isDragging = false;


            AfterDrag();
        }

        private void AfterDrag()
        {
            if (_dragWnd != null && _dragWnd.NeedReCreate)
                _dragWnd.Recreate();
            _dragWnd = null;
            _rootTarget.CloseDropWindow();
            _rootTarget = null;
            DragTarget = null;
            _DestroyDragItem();
        }
        #endregion

        #region init & destroy
        private void _InitDragItem()
        {
            ILayoutGroup group;
            IDockElement ele;
            var mouseP = DockHelper.GetMousePosition(DockManager);
            switch (_dragItem.Mode)
            {
                case DockMode.Normal:
                    if (_dragItem.RelativeObj is ILayoutGroup)
                    {
                        var _layoutGroup = _dragItem.RelativeObj as ILayoutGroup;
                        //这里移动的一定是AnchorSideGroup，故将其从父级LayoutGroupPanel移走，但不Dispose留着构造浮动窗口
                        if ((_layoutGroup.View as ILayoutGroupControl).TryDeatchFromParent(false))
                        {
                            //注意重新设置Mode
                            _layoutGroup.SetDockMode(DockMode.Float);
                            _dragWnd = new AnchorGroupWindow()
                            {
                                Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - 1,
                                Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - 1
                            };
                            _dragWnd.AttachChild(_layoutGroup.View, 0);
                            _dragWnd.Show();
                        }
                    }
                    else if (_dragItem.RelativeObj is IDockElement)
                    {
                        ele = _dragItem.RelativeObj as IDockElement;
                        if (ele.IsDocument)
                            group = new LayoutDocumentGroup(DockManager);
                        else group = new LayoutGroup(ele.Side, DockManager);
                        //先从逻辑父级中移除
                        ele.Container.Detach(ele);
                        //再加入新的逻辑父级
                        group.Attach(ele);
                        //注意重新设置Mode
                        group.SetDockMode(DockMode.Float);
                        //创建新的浮动窗口，并初始化位置
                        //这里可知引起drag的时DragTabItem故这里创建临时的DragTabWindow
                        if (ele.IsDocument)
                        {
                            _dragWnd = new DocumentGroupWindow() { NeedReCreate = true };
                            _dragWnd.AttachChild(new LayoutDocumentGroupControl(group) { IsDraggingFromDock = true, BorderThickness = new Thickness(1) }, 0);
                            _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                            _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                        }
                        else
                        {
                            _dragWnd = new AnchorGroupWindow() { NeedReCreate = true };
                            _dragWnd.AttachChild(new AnchorSideGroupControl(group) { IsDraggingFromDock = true }, 0);
                            _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - ele.DesiredHeight + 20;
                            _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength;
                        }
                        _dragWnd.Background = Brushes.Transparent;
                        _dragWnd.BorderBrush = Brushes.Transparent;
                        _dragWnd.Show();
                    }
                    break;
                case DockMode.DockBar:
                    //这里表示从自动隐藏窗口进行的拖动，因此这里移除自动隐藏窗口
                    ele = _dragItem.RelativeObj as IDockElement;
                    ele.Container.Detach(ele);
                    //创建新的浮动窗口，并初始化位置
                    group = new LayoutGroup(ele.Side, DockManager);
                    group.Attach(ele);
                    //注意重新设置Mode
                    group.SetDockMode(DockMode.Float);
                    _dragWnd = new AnchorGroupWindow()
                    {
                        Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - 1,
                        Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - 1
                    };
                    _dragWnd.AttachChild(new AnchorSideGroupControl(group), 0);
                    _dragWnd.Show();
                    break;
                case DockMode.Float:
                    if (_dragItem.RelativeObj is IDockElement)
                    {
                        ele = _dragItem.RelativeObj as IDockElement;
                        var ctrl = ele.Container.View as BaseGroupControl;
                        if (ctrl.Items.Count == 1 && ctrl.Parent is BaseFloatWindow)
                        {
                            _dragWnd = (ctrl.Parent as BaseFloatWindow);
                            _dragWnd.Hide();
                            (ctrl.Parent as BaseFloatWindow).Recreate();
                            _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                            _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            _dragWnd.Show();
                        }
                        else
                        {
                            if (ele.Container is LayoutDocumentGroup)
                                group = new LayoutDocumentGroup(DockManager);
                            else group = new LayoutGroup(ele.Side, DockManager);
                            //先从逻辑父级中移除
                            ele.Container.Detach(ele);
                            //再加入新的逻辑父级
                            group.Attach(ele);
                            //创建新的浮动窗口，并初始化位置
                            //这里可知引起drag的时DragTabItem故这里创建临时的DragTabWindow
                            if (ele.IsDocument)
                            {
                                _dragWnd = new DocumentGroupWindow() { NeedReCreate = true };
                                _dragWnd.AttachChild(new LayoutDocumentGroupControl(group) { IsDraggingFromDock = true, BorderThickness = new Thickness(1) }, 0);
                                _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                                _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            }
                            else
                            {
                                _dragWnd = new AnchorGroupWindow() { NeedReCreate = true };
                                _dragWnd.AttachChild(new AnchorSideGroupControl(group) { IsDraggingFromDock = true }, 0);
                                _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                                _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            }
                            _dragWnd.Background = Brushes.Transparent;
                            _dragWnd.BorderBrush = Brushes.Transparent;
                            _dragWnd.Show();
                        }
                    }
                    else if (_dragItem.RelativeObj is ILayoutGroup)
                    {
                        group = _dragItem.RelativeObj as ILayoutGroup;
                        //表示此时的浮动窗口为IsSingleMode
                        if (group.View.DockViewParent == null)
                            _dragWnd = (group.View as BaseGroupControl).Parent as BaseFloatWindow;
                        else
                        {
                            //这里移动的一定是AnchorSideGroup，故将其从父级LayoutGroupPanel移走，但不Dispose留着构造浮动窗口
                            if ((group.View as ILayoutGroupControl).TryDeatchFromParent(false))
                            {
                                _dragWnd = new AnchorGroupWindow()
                                {
                                    Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - 1,
                                    Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - 1
                                };
                                _dragWnd.AttachChild(group.View, 0);
                                _dragWnd.Show();
                            }
                        }
                    }
                    else if (_dragItem.RelativeObj is BaseFloatWindow)
                        _dragWnd = _dragItem.RelativeObj as BaseFloatWindow;
                    break;
            }
        }

        private void _DestroyDragItem()
        {
            _dragItem.Dispose();
            _dragItem = null;
            _dragTarget = null;
        }
        #endregion
        #endregion

        #region Flag
        internal const int LEFT = 0x0001;
        internal const int TOP = 0x0002;
        internal const int RIGHT = 0x0004;
        internal const int BOTTOM = 0x0008;
        internal const int CENTER = 0x0010;
        internal const int SPLIT = 0x1000;
        #endregion

        #region DragEvent
        internal void OnMouseMove(object sender)
        {
            var p = DockHelper.GetMousePositionRelativeTo(DockManager.LayoutRootPanel.RootGroupPanel);
            if (_rootRect.Contains(p))
            {
                if (_rootTarget.IsDragWndHide)
                    _rootTarget.ShowDropWindow();
                VisualTreeHelper.HitTest(DockManager.LayoutRootPanel.RootGroupPanel, _HitFilter, _HitRessult, new PointHitTestParameters(p));
            }
            else _rootTarget.HideDropWindow();
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
                DragTarget = potentialHitTestTarget as IDragTarget;
                return HitTestFilterBehavior.Stop;
            }
            return HitTestFilterBehavior.Continue;
        }
        #endregion
    }
}