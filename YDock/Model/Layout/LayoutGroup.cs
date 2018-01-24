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
    public class LayoutGroup : ILayoutGroup
    {
        public LayoutGroup(DockSide side, YDock dockManager)
        {
            _side = side;
            _dockManager = dockManager;
            _children.CollectionChanged += OnChildrenCollectionChanged;
        }

        protected virtual void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnChildrenPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanSelect")
            {
                if (!(sender as LayoutElement).CanSelect)
                {
                    //移出的元素排到最后一个
                    _children.Remove(sender as LayoutElement);
                    _children.Add(sender as LayoutElement);
                }
                else
                {
                    //重新进入的元素排到第一个
                    _children.Remove(sender as LayoutElement);
                    _children.Insert(0, sender as LayoutElement);
                }
                //OnChildrenChanged方法一定会触发下面的事件，故这里不用重复触发
                //PropertyChanged(this, new PropertyChangedEventArgs("Children_CanSelect"));
            }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private DockSide _side;
        public DockSide Side { get { return _side; } }

        protected ObservableCollection<ILayoutElement> _children = new ObservableCollection<ILayoutElement>();
        public ObservableCollection<ILayoutElement> Children
        {
            get { return _children; }
        }

        IEnumerable<ILayoutElement> ILayoutGroup.Children
        {
            get
            {
                return _children;
            }
        }

        public IEnumerable<LayoutElement> Children_CanSelect
        {
            get
            {
                foreach (LayoutElement child in _children)
                    if (child.CanSelect)
                        yield return child;
            }
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

        private YDock _dockManager;
        public YDock DockManager
        {
            get
            {
                return _dockManager;
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
                {
                    if (_view != null)
                        _view.Model = null;
                    _view = value;
                    if (_view != null)
                        _view.Model = this;
                }
            }
        }
    }

    public class LayoutDocumentGroup : LayoutGroup
    {
        public LayoutDocumentGroup(YDock dockManager) : base(DockSide.None, dockManager)
        {

        }

        protected override void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnChildrenCollectionChanged(sender, e);
            RaisePropertyChanged("ChildrenSorted");
        }

        public IEnumerable<ILayoutElement> ChildrenSorted
        {
            get
            {
                var listSorted = Children_CanSelect.ToList();
                listSorted.Sort();
                return listSorted;
            }
        }
    }
}