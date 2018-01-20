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
                    item.PropertyChanged += OnChildrenPropertyChanged;
                }
        }
        private void OnChildrenPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanSelect")
                PropertyChanged(this, new PropertyChangedEventArgs("Children_CanSelect"));
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
                    _side = value;
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