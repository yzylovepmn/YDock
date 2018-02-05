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
        private ILayoutGroupControl _dragTarget;
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
                            _dragWnd = new SingleAnchorWindow()
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
                            _dragWnd = new SingleAnchorWindow() { NeedReCreate = true };
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
                    _dragWnd = new SingleAnchorWindow()
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
                            if (ele.IsDocument)
                            {
                                _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                                _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            }
                            else
                            {
                                _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                                _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            }
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
                                _dragWnd = new SingleAnchorWindow() { NeedReCreate = true };
                                _dragWnd.AttachChild(new AnchorSideGroupControl(group) { IsDraggingFromDock = true }, 0);
                                _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowHeaderHeight - Constants.FloatWindowResizeLength;
                                _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding;
                            }
                            _dragWnd.Background = Brushes.Transparent;
                            _dragWnd.BorderBrush = Brushes.Transparent;
                            _dragWnd.Show();
                        }
                    }
                    //else if (_dragItem.RelativeObj is ILayoutGroup)
                    //{
                    //    group = _dragItem.RelativeObj as ILayoutGroup;
                    //    //表示此时的Parent为SingleAnchorWindow
                    //    if (group.View.DockViewParent == null)
                    //        _dragWnd = (group.View as FrameworkElement).Parent as BaseFloatWindow;
                    //    else
                    //    {

                    //    }
                    //}
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

        #region DragEvent
        internal void OnMouseMove(object sender)
        {
            var p = DockHelper.GetMousePositionRelativeTo(DockManager.LayoutRootPanel.RootGroupPanel);
            VisualTreeHelper.HitTest(DockManager.LayoutRootPanel.RootGroupPanel, _HitFilter, _HitRessult, new PointHitTestParameters(p));
        }

        private HitTestResultBehavior _HitRessult(HitTestResult result)
        {
            _dragTarget = null;
            return HitTestResultBehavior.Stop;
        }

        private HitTestFilterBehavior _HitFilter(DependencyObject potentialHitTestTarget)
        {
            if (potentialHitTestTarget is ILayoutGroupControl)
            {
                _dragTarget = potentialHitTestTarget as ILayoutGroupControl;
                return HitTestFilterBehavior.Stop;
            }
            return HitTestFilterBehavior.Continue;
        }

        private void _CreatDragWindow()
        {

        }
        #endregion
    }
}