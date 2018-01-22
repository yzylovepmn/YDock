using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using YDock.Enum;
using YDock.Interface;

namespace YDock.Model
{
    [ContentProperty("Children")]
    public class YDockSide : IModel, ILayoutGroup
    {
        public YDockSide()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (LayoutElement item in e.OldItems)
                {
                    item.PropertyChanged -= OnChildrenPropertyChanged;
                    item.Container = null;
                }
            if (e.NewItems != null)
                foreach (LayoutElement item in e.NewItems)
                {
                    item.Container = this;
                    item.Side = _side;
                    item.PropertyChanged += OnChildrenPropertyChanged;
                }
            PropertyChanged(this, new PropertyChangedEventArgs("Children_CanSelect"));
        }

        private void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanSelect")
            {
                if (!(sender as LayoutElement).CanSelect)
                {
                    _children.Remove(sender as LayoutElement);
                    _children.Add(sender as LayoutElement);
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Children_CanSelect"));
            }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void MoveTo(int src, int des)
        {
            if (src < Children.Count && src >= 0
                && des < Children.Count && des >= 0)
            {
                var child = Children[src];
                Children.RemoveAt(src);
                Children.Insert(des, child);
            }
        }

        public int IndexOf(ILayoutElement child)
        {
            return Children.IndexOf(child as LayoutElement);
        }

        #region Root
        private YDockRoot _root;
        public YDockRoot Root
        {
            get { return _root; }
            set
            {
                if (_root != value)
                    _root = value;
            }
        }
        #endregion

        #region Side
        private DockSide _side;
        public DockSide Side
        {
            get { return _side; }
            set
            {
                if (_side != value)
                {
                    _side = value;
                    foreach (var child in Children)
                        child.Side = value;
                }
            }
        }
        private IView _view;
        public IView View
        {
            get
            {
                return _view;
            }
            set
            {
                if (_view != value)
                    _view = value;
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        ObservableCollection<LayoutElement> _children = new ObservableCollection<LayoutElement>();
        public ObservableCollection<LayoutElement> Children
        {
            get { return _children; }
        }

        public IEnumerable<LayoutElement> Children_CanSelect
        {
            get
            {
                foreach (var child in _children)
                    if (child.CanSelect)
                        yield return child;
            }
        }


        public YDock DockManager
        {
            get
            {
                return _root.DockManager;
            }
        }

        IEnumerable<ILayoutElement> ILayoutGroup.Children
        {
            get
            {
                foreach (var child in _children)
                    yield return child;
            }
        }
    }
}