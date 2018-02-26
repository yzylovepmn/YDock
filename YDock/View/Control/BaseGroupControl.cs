using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class BaseGroupControl : TabControl, ILayoutGroupControl, IDragTarget
    {
        internal BaseGroupControl(ILayoutGroup model)
        {
            Model = model;
            SetBinding(ItemsSourceProperty, new Binding("Model.Children_CanSelect") { Source = this });
            if (model.Children.Count() > 0)
            {
                DesiredWidth = model.Children.First().DesiredWidth;
                DesiredHeight = model.Children.First().DesiredHeight;
            }
        }

        #region for drag
        internal IDockElement _dragItem;
        internal IList<Rect> _childrenBounds;
        internal bool _mouseInside = true;
        internal Point _mouseDown;
        internal Rect _rect;

        internal void UpdateChildrenBounds(Panel parent)
        {
            _childrenBounds = new List<Rect>();
            double hoffset = 0;
            foreach (TabItem child in parent.Children)
            {
                var childSize = child.TransformActualSizeToAncestor();
                _childrenBounds.Add(new Rect(new Point(hoffset, 0), childSize));
                hoffset += childSize.Width;
            }
        }
        #endregion

        private double _desiredWidth;
        public double DesiredWidth
        {
            get
            {
                return _desiredWidth;
            }
            set
            {
                if (_desiredWidth != value)
                    _desiredWidth = value;
            }
        }

        private double _desiredHeight;
        public double DesiredHeight
        {
            get
            {
                return _desiredHeight;
            }
            set
            {
                if (_desiredHeight != value)
                    _desiredHeight = value;
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.RemovedItems != null)
                foreach (DockElement item in e.RemovedItems)
                    item.IsVisible = false;

            if (e.AddedItems != null)
                foreach (DockElement item in e.AddedItems)
                    item.IsVisible = true;
        }

        private IDockModel _model;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public IDockModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model != value)
                {
                    if (_model != null)
                    {
                        (_model as LayoutGroup).View = null;
                        _model.DockManager.DragManager.OnDragStatusChanged -= OnDragStatusChanged;
                    }
                    _model = value;
                    if (_model != null)
                    {
                        (_model as LayoutGroup).View = this;
                        _model.DockManager.DragManager.OnDragStatusChanged += OnDragStatusChanged;
                    }
                }
            }
        }

        private bool _isDraggingFromDock = false;
        public bool IsDraggingFromDock
        {
            get { return _isDraggingFromDock; }
            set
            {
                if (_isDraggingFromDock != value)
                {
                    _isDraggingFromDock = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsDraggingFromDock"));
                }
            }
        }

        public IDockView DockViewParent
        {
            get
            {
                return Parent as IDockView;
            }
        }

        public virtual DragMode Mode
        {
            get;
        }

        public DockManager DockManager
        {
            get
            {
                return _model.DockManager;
            }
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (SelectedContent != null)
                (_model as ILayoutGroup).SetActive(SelectedContent as DockElement);
        }

        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            if (SelectedContent != null)
                (_model as ILayoutGroup).SetActive(SelectedContent as DockElement);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DragTabItem(this);
        }

        public bool TryDeatchFromParent(bool isDispose = true)
        {
            if (Parent != null)
            {
                if (DockViewParent is ILayoutPanel)
                {
                    var panel = DockViewParent as ILayoutPanel;
                    if (panel.IsDocumentPanel && panel.Count == 1)
                        return false;
                }
                (Parent as ILayoutViewParent).DetachChild(this);
                DesiredHeight = ActualHeight;
                DesiredWidth = ActualWidth;
                if (isDispose)
                    Dispose();
            }
            return true;
        }

        public void AttachToParent(ILayoutPanel parent, int index)
        {
            parent.AttachChild(this, index);
        }

        public virtual void Dispose()
        {
            BindingOperations.ClearBinding(this, ItemsSourceProperty);
            Items.Clear();
            Model = null;
            _dragItem = null;
            _childrenBounds.Clear();
            _childrenBounds = null;
        }


        DropWindow _dragWnd;
        bool _isFirstShow;
        public virtual void OnDrop(DragItem source, int flag)
        {
            
        }

        internal virtual void CreateDropWindow()
        {
            if (_dragWnd == null)
            {
                _isFirstShow = true;
                _dragWnd = new DropWindow(this);
            }
        }

        public void CloseDropWindow()
        {
            if (_dragWnd != null)
            {
                _dragWnd.IsOpen = false;
                _dragWnd = null;
            }
        }

        public void HideDropWindow()
        {
            _dragWnd?.Hide();
        }

        public void ShowDropWindow()
        {
            if (_dragWnd == null)
            {
                CreateDropWindow();
                _dragWnd.IsOpen = true;
            }
            if (_isFirstShow)
            {
                _isFirstShow = false;
                var p = this.PointToScreenDPIWithoutFlowDirection(new Point());
                DockHelper.UpdateLocation(_dragWnd, p.X, p.Y, ActualWidth, ActualHeight);
            }
            _dragWnd.Show();
        }

        public void Update()
        {
            _dragWnd?.Update();
        }

        private void OnDragStatusChanged(DragStatusChangedEventArgs args)
        {
            if (Parent is BaseFloatWindow && (Parent as BaseFloatWindow).IsDragging)
                return;
            if (!args.IsDragging)
                CloseDropWindow();
        }
    }
}