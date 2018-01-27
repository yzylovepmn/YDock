using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YDock.Enum;
using YDock.Interface;

namespace YDock.Model
{
    public abstract class BaseLayoutGroup : ILayoutGroup
    {
        public BaseLayoutGroup()
        {
            _children.CollectionChanged += OnChildrenCollectionChanged;
        }

        protected virtual void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (DockElement item in e.OldItems)
                {
                    item.PropertyChanged -= OnChildrenPropertyChanged;
                    item.Container = null;
                }
            if (e.NewItems != null)
                foreach (DockElement item in e.NewItems)
                {
                    item.Container = this;
                    item.PropertyChanged += OnChildrenPropertyChanged;
                }
            PropertyChanged(this, new PropertyChangedEventArgs("Children_CanSelect"));
        }

        protected virtual void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanSelect")
            {
                _children.CollectionChanged -= OnChildrenCollectionChanged;
                if (!(sender as DockElement).CanSelect)
                {
                    //移出的元素排到最后一个
                    _children.Remove(sender as DockElement);
                    _children.Add(sender as DockElement);
                }
                else
                {
                    //重新进入的元素排到第一个
                    _children.Remove(sender as DockElement);
                    _children.Insert(0, sender as DockElement);
                }
                _children.CollectionChanged += OnChildrenCollectionChanged;
                RaisePropertyChanged("Children_CanSelect");
            }
        }

        protected ObservableCollection<IDockElement> _children = new ObservableCollection<IDockElement>();
        public ObservableCollection<IDockElement> Children
        {
            get { return _children; }
        }

        public IEnumerable<DockElement> Children_CanSelect
        {
            get
            {
                foreach (DockElement child in _children)
                    if (child.CanSelect)
                        yield return child;
            }
        }

        IEnumerable<IDockElement> ILayoutGroup.Children
        {
            get { return _children; }
        }

        public abstract DockManager DockManager
        {
            get;
        }

        protected DockSide _side;
        public DockSide Side
        {
            get { return _side; }
            internal set
            {
                if (_side != value)
                {
                    _side = value;
                    foreach (DockElement child in Children)
                        child.Side = value;
                }
            }
        }

        protected IDockView _view;
        public IDockView View
        {
            get
            {
                return _view;
            }

            internal set
            {
                if (_view != value)
                    _view = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int IndexOf(IDockElement child)
        {
            return Children.IndexOf(child as DockElement);
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

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Dispose()
        {
            _children.CollectionChanged -= OnChildrenCollectionChanged;
            foreach (var child in _children)
                child.PropertyChanged -= OnChildrenPropertyChanged;
            _children.Clear();
            PropertyChanged = null;
            _view = null;
        }
    }
}