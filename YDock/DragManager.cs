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

        private DockManager _dockManager;
        public DockManager DockManager
        {
            get { return _dockManager; }
        }

        private ILayoutGroupControl _dragTarget;
        private DragItem _dragItem;
        private Window _dragHelper;
        private BaseFloatWindow _dragWnd;
        private bool _isDragging = false;
        public bool IsDragging
        {
            get { return _isDragging; }
        }


        internal void IntoDragAction(DragItem dragItem)
        {
            _dragItem = dragItem;
            BeforeDrag();
        }

        private void BeforeDrag()
        {
            _isDragging = true;
            _InitDragItem();
            _InitDragHelper();
        }

        private void _DoDragDrop()
        {
            _isDragging = false;


            AfterDrag();
        }

        private void AfterDrag()
        {
            if (_dragWnd is DragTabWindow)
                _dragWnd.Close();
            else _dragWnd = null;
            _DestroyDragHelper();
            _DestroyDragItem();
        }

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
                        if (ele.Container is LayoutDocumentGroup)
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
                        _dragWnd = DragTabWindow.CreateDragTabWindow(_dragItem);
                        _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y;
                        _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left;
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
                        if (ele.Container is LayoutDocumentGroup)
                            group = new LayoutDocumentGroup(DockManager);
                        else group = new LayoutGroup(ele.Side, DockManager);
                        //先从逻辑父级中移除
                        ele.Container.Detach(ele);
                        //再加入新的逻辑父级
                        group.Attach(ele);
                        //创建新的浮动窗口，并初始化位置
                        //这里可知引起drag的时DragTabItem故这里创建临时的DragTabWindow
                        _dragWnd = DragTabWindow.CreateDragTabWindow(_dragItem);
                        _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y;
                        _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left;
                        _dragWnd.Show();
                    }
                    else if (_dragItem.RelativeObj is ILayoutGroup)
                    {
                        group = _dragItem.RelativeObj as ILayoutGroup;
                        //表示此时的Parent为SingleAnchorWindow
                        if (group.View.DockViewParent == null)
                            _dragWnd = (group.View as FrameworkElement).Parent as BaseFloatWindow;
                        else
                        {

                        }
                    }
                    break;
            }
        }

        private void _DestroyDragItem()
        {
            _dragItem.Dispose();
            _dragItem = null;
            _dragTarget = null;
        }

        private void _InitDragHelper()
        {
            _dragHelper = DockHelper.CreateTransparentWindow();
            _dragHelper.MouseMove += OnMouseMove;
            _dragHelper.MouseLeftButtonUp += OnMouseLeftButtonUp;
            _dragHelper.Show();
        }

        private void _DestroyDragHelper()
        {
            _dragHelper.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            _dragHelper.MouseMove -= OnMouseMove;
            _dragHelper.Close();
            _dragHelper = null;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _UpdateDragWndPos(DockHelper.GetMousePosition(DockManager.LayoutRootPanel));
            var p = e.GetPosition(DockManager.LayoutRootPanel.RootGroupPanel);
            VisualTreeHelper.HitTest(DockManager.LayoutRootPanel.RootGroupPanel, _HitFilter, _HitRessult, new PointHitTestParameters(p));
        }

        void _UpdateDragWndPos(Point mouseP)
        {
            if (_dragWnd != null)
            {
                if (_dragWnd is DocumentGroupWindow)
                {
                    _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragWnd.WidthEceeed / 2 - 1;
                    _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - Constants.FloatWindowHeaderHeight - 1;
                }
                else if (_dragWnd is AnchorGroupWindow)
                {
                    _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - 1;
                    _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - Constants.FloatWindowHeaderHeight - 1;
                }
                else if (_dragWnd is SingleAnchorWindow)
                {
                    _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - Constants.FloatWindowResizeLength - 1;
                    _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y - Constants.FloatWindowResizeLength - 1;
                }
                else if (_dragWnd is DragTabWindow)
                {
                    _dragWnd.Top = mouseP.Y - _dragItem.ClickPos.Y;
                    _dragWnd.Left = mouseP.X - _dragItem.ClickPos.X - _dragItem.ClickRect.Left;
                }
            }
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

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _DoDragDrop();
        }
    }
}