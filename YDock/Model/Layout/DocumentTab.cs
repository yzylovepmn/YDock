using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using YDock.Interface;

namespace YDock.Model
{
    public class DocumentTab : IModel, ILayoutContainer
    {
        public DocumentTab(IModel parent)
        {
            _parent = parent;
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (ILayoutElement item in e.OldItems)
                {
                    item.PropertyChanged -= OnChildrenPropertyChanged;
                    item.Container = null;
                }
            if (e.NewItems != null)
                foreach (ILayoutElement item in e.NewItems)
                {
                    item.Container = this;
                    item.PropertyChanged += OnChildrenPropertyChanged;
                }
        }

        private void OnChildrenPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanSelect")
                PropertyChanged(this, new PropertyChangedEventArgs("Children_CanSelect"));
        }

        ObservableCollection<ILayoutElement> _children = new ObservableCollection<ILayoutElement>();
        public ObservableCollection<ILayoutElement> Children
        {
            get { return _children; }
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private IModel _parent;
        public IModel Parent
        {
            get { return _parent; }
        }

        public IEnumerable<ILayoutElement> ChildrenSorted
        {
            get
            {
                var listSorted = Children.ToList();
                listSorted.Sort();
                return listSorted;
            }
        }

        IEnumerable<ILayoutElement> ILayoutContainer.Children
        {
            get
            {
                return Children;
            }
        }

        public YDock DockManager
        {
            get
            {
                return _parent.DockManager;
            }
        }
    }
}