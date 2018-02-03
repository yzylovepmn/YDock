using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class BaseGroupControl : TabControl, ILayoutGroupControl
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
                        (_model as LayoutGroup).View = null;
                    _model = value;
                    if (_model != null)
                        (_model as LayoutGroup).View = this;
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
    }
}