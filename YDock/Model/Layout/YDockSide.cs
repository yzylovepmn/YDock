using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using YDock.Enum;
using YDock.Interface;

namespace YDock.Model
{
    [ContentProperty("Children")]
    public class YDockSide : IAnchorModel, ILayoutContainer
    {
        public YDockSide()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (ILayoutElement item in e.OldItems)
                    item.Container = null;
            if (e.NewItems != null)
                foreach (ILayoutElement item in e.NewItems)
                    item.Container = this;
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

        ObservableCollection<LayoutElement> _children = new ObservableCollection<LayoutElement>();
        public ObservableCollection<LayoutElement> Children
        {
            get { return _children; }
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
                return _root.DockManager;
            }
        }
    }
}