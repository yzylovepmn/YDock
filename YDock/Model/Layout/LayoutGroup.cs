using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                foreach (var child in _children)
                    yield return child;
            }
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

        public IEnumerable<LayoutElement> Children_CanSelect
        {
            get
            {
                foreach (LayoutElement child in _children)
                    if (child.CanSelect)
                        yield return child;
            }
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