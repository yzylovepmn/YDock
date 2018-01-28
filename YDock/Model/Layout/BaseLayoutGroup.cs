using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
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
                    item.Container = null;
                    item.PropertyChanged -= OnChildrenPropertyChanged;
                }
            if (e.NewItems != null)
                foreach (DockElement item in e.NewItems)
                {
                    item.Container = this;
                    item.Side = _side;
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
                    _children.Insert(Children_CanSelect.Count(), sender as DockElement);
                }
                _children.CollectionChanged += OnChildrenCollectionChanged;
                RaisePropertyChanged("Children_CanSelect");
            }
        }

        protected ObservableCollection<IDockElement> _children = new ObservableCollection<IDockElement>();
        public IEnumerable<IDockElement> Children
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
                    foreach (DockElement child in _children)
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
            if (child == null) return -1;
            return _children.IndexOf(child as DockElement);
        }

        public void MoveTo(int src, int des)
        {
            if (src < _children.Count && src >= 0
                && des < _children.Count && des >= 0)
                _children.Move(src, des);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void SetActive(IDockElement element)
        {
            if (_view != null)
            {
                if (element != null && !element.CanSelect)
                    (element as DockElement).CanSelect = true;
                DockManager.ActiveElement = element as DockElement;
            }
        }

        public virtual void Detach(IDockElement element)
        {
            if (element == null || !_children.Contains(element))
                throw new InvalidOperationException("Detach Failed!");
            _children.Remove(element);
        }

        public virtual void Attach(IDockElement element)
        {
            if (element == null || element.Container != null)
                throw new InvalidOperationException("Attach Failed!");
            _children.Add(element);
        }

        public virtual void Dispose()
        {
            _children.CollectionChanged -= OnChildrenCollectionChanged;
            foreach (var child in _children)
            {
                child.PropertyChanged -= OnChildrenPropertyChanged;
                (child as DockElement).Container = null;
            }
            _children.Clear();
            _children = null;
            PropertyChanged = null;
            _view = null;
        }
    }
}