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
        public DragManager(DockManager dockManager)
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
            _DestroyDragHelper();
            _DestroyDragItem();
        }

        private void _InitDragItem()
        {

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
            _dragHelper.Owner = DockManager.MainWindow;
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
            var p = e.GetPosition(DockManager.LayoutRootPanel.RootGroupPanel);
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

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _DoDragDrop();
        }
    }
}