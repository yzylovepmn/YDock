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
                        if (_model.DockManager != null)
                            _model.DockManager.DragManager.OnDragStatusChanged -= OnDragStatusChanged;
                    }
                    _model = value;
                    if (_model != null)
                    {
                        (_model as LayoutGroup).View = this;
                        if (_model.DockManager != null)
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


        public int ChildrenCount
        {
            get
            {
                if (DockViewParent is LayoutGroupPanel)
                    return (DockViewParent as LayoutGroupPanel).Count;
                else return 1;
            }
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

        public int IndexOf()
        {
            if (DockViewParent is LayoutGroupPanel)
                return (DockViewParent as LayoutGroupPanel).Children.IndexOf(this);
            else return -1;
        }

        public virtual void Dispose()
        {
            BindingOperations.ClearBinding(this, ItemsSourceProperty);
            Items.Clear();
            Model = null;
            _dragItem = null;
            _childrenBounds?.Clear();
            _childrenBounds = null;
        }

        #region Drag
        private DropMode _dropMode = DropMode.None;
        public DropMode DropMode
        {
            get { return _dropMode; }
            set { _dropMode = value; }
        }
        DropWindow _dragWnd;
        public virtual void OnDrop(DragItem source)
        {
            var child = (source.RelativeObj as BaseFloatWindow).Child;
            (source.RelativeObj as BaseFloatWindow).DetachChild(child);

            DockManager.ChangeSide(child, Model.Side);

            var group = Model as LayoutGroup;
            switch (DropMode)
            {
                case DropMode.Header:
                case DropMode.Center:
                    _AttachDockView(child as UIElement, group);
                    break;
            }
        }

        private void _AttachDockView(UIElement view, LayoutGroup target)
        {
            if (view is LayoutGroupPanel)
                foreach (UIElement item in (view as LayoutGroupPanel).Children)
                    _AttachDockView(item, target);

            if (view is BaseGroupControl)
            {
                var model = (view as BaseGroupControl).Model as LayoutGroup;
                var _children = new List<IDockElement>(model.Children.Reverse());
                model.Dispose();
                foreach (var item in _children)
                    target.Attach(item, _index);
            }

            if (view is IDisposable)
                (view as IDisposable).Dispose();
        }

        internal virtual void CreateDropWindow()
        {
            if (_dragWnd == null)
                _dragWnd = new DropWindow(this);
        }

        public void CloseDropWindow()
        {
            if (_dragWnd != null)
            {
                _dropMode = DropMode.None;
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
                CreateDropWindow();
            var p = this.PointToScreenDPIWithoutFlowDirection(new Point());
            if (this is LayoutDocumentGroupControl)
            {
                if (DockViewParent == null)
                {
                    _dragWnd.DropPanel.InnerRect = new Rect(0, 0, ActualWidth, ActualHeight);
                    DockHelper.UpdateLocation(_dragWnd, p.X, p.Y, ActualWidth, ActualHeight);
                }
                else
                {
                    var panel = DockViewParent as LayoutGroupDocumentPanel;
                    var p1 = panel.PointToScreenDPIWithoutFlowDirection(new Point());
                    _dragWnd.DropPanel.InnerRect = new Rect(p.X - p1.X, p.Y - p1.Y, ActualWidth, ActualHeight);
                    DockHelper.UpdateLocation(_dragWnd, p.X, p.Y, panel.ActualWidth, panel.ActualHeight);
                }
            }
            else DockHelper.UpdateLocation(_dragWnd, p.X, p.Y, ActualWidth, ActualHeight);
            if (!_dragWnd.IsOpen) _dragWnd.IsOpen = true;
            _dragWnd.Show();
        }

        public void Update(Point mouseP)
        {
            _dragWnd?.Update(mouseP);
        }

        internal void OnDragStatusChanged(DragStatusChangedEventArgs args)
        {
            if (!args.IsDragging)
                CloseDropWindow();
        }
        #endregion

        #region HitTest
        internal bool canUpdate = true;
        internal int _index;
        private ActiveRectDropVisual _activeRect;
        Point _mouseP;
        public void HitTest(Point mouseP, ActiveRectDropVisual activeRect)
        {
            _mouseP = mouseP;
            _activeRect = activeRect;
            _activeRect.Flag = DragManager.NONE;
            var p = this.PointToScreenDPIWithoutFlowDirection(new Point());
            p = new Point(mouseP.X - p.X, mouseP.Y - p.Y);
            VisualTreeHelper.HitTest(this, _HitFilter, _HitRessult, new PointHitTestParameters(p));
            _activeRect = null;
        }

        private HitTestFilterBehavior _HitFilter(DependencyObject potentialHitTestTarget)
        {
            canUpdate = true;
            if (potentialHitTestTarget is AnchorHeaderControl && _activeRect.DropPanel.Source.DragMode != DragMode.Document)
            {
                if (DropMode == DropMode.Center && _index == -1)
                {
                    canUpdate = false;
                    return HitTestFilterBehavior.Stop;
                }
                _activeRect.Flag = DragManager.HEAD;
                _activeRect.Rect = new Rect(0, 0, 60, 20);
                _index = -1;
            }
            else if (potentialHitTestTarget is AnchorSidePanel
                || potentialHitTestTarget is DocumentPanel)
            {
                UpdateChildrenBounds(potentialHitTestTarget as Panel);
                var p = (potentialHitTestTarget as Panel).PointToScreenDPIWithoutFlowDirection(new Point());
                var index = _childrenBounds.FindIndex(new Point(_mouseP.X - p.X, _mouseP.Y - p.Y));
                Rect rect;
                if (index < 0)
                {
                    if (_childrenBounds.Count == 0)
                        rect = new Rect(0, 0, 0, 22);
                    else
                    {
                        rect = _childrenBounds.Last();
                        rect = new Rect(rect.X + rect.Width, 0, 0, rect.Height);
                    }
                    if (DropMode == DropMode.Center && _index == _childrenBounds.Count)
                    {
                        canUpdate = false;
                        return HitTestFilterBehavior.Stop;
                    }
                    _index = _childrenBounds.Count;
                }
                else
                {
                    rect = _childrenBounds[index];
                    if (DropMode == DropMode.Center && _index == index)
                    {
                        canUpdate = false;
                        return HitTestFilterBehavior.Stop;
                    }
                    _index = index;
                }
                _activeRect.Flag = DragManager.CENTER;
                _activeRect.Rect = new Rect(rect.X, 0, this is AnchorSideGroupControl ? 60 : 120, rect.Height);

                return HitTestFilterBehavior.Stop;
            }
            return HitTestFilterBehavior.Continue;
        }

        private HitTestResultBehavior _HitRessult(HitTestResult result)
        {
            return HitTestResultBehavior.Stop;
        }
        #endregion
    }
}