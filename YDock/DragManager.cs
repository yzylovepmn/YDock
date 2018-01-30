using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;
using YDock.View;

namespace YDock
{
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

        private ILayoutGroup _dragItem;
        private Window _dragWnd;
        private Window _dragHelper;
        private bool _isDragging = false;
        public bool IsDragging
        {
            get { return _isDragging; }
        }


        internal void IntoDragAction(object relativeObj)
        {
            BeforeDrag(relativeObj);
        }

        private void BeforeDrag(object relativeObj)
        {
            _isDragging = true;
            _InitDragItem(relativeObj);
            _InitDragHelper();
        }

        private void _DoDragDrop()
        {

            AfterDrag();
        }

        private void AfterDrag()
        {
            _DestroyDragHelper();
            _DestroyDragItem();
            _isDragging = false;
        }

        private void _InitDragItem(object relativeObj)
        {
            if (relativeObj is ILayoutGroup)
                _dragItem = relativeObj as ILayoutGroup;
            if (relativeObj is IDockElement)
            {
                IDockElement ele = relativeObj as IDockElement;
                ele.Container.Detach(ele);
                if (ele.Side == DockSide.None)
                {
                    _dragItem = new LayoutDocumentGroup(DockManager);
                    _dragItem.Attach(ele);
                }
                else
                {
                    _dragItem = new LayoutGroup(ele.Side, DockManager);
                    _dragItem.Attach(ele);
                }
            }
            IDockView view;
            if (_dragItem.View == null)
            {
                if (_dragItem is LayoutDocumentGroup)
                    view = new LayoutDocumentGroupControl(_dragItem);
                else view = new AnchorSideGroupControl(_dragItem);
            }
            else
            {
                view = _dragItem.View;
                (view as ILayoutGroupControl).TryDeatchFromParent(false);
            }
            AnchorGroupWindow window = new AnchorGroupWindow();
            window.AttachChild(view, 0);
            window.Show();
        }

        private void _DestroyDragItem()
        {
            _dragItem = null;
            _dragWnd = null;
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
            _dragHelper.MouseMove -= OnMouseMove;
            _dragHelper.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            _dragHelper.Close();
            _dragHelper = null;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _DoDragDrop();
        }
    }
}