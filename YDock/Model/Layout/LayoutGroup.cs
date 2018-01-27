using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YDock.Enum;
using YDock.Interface;
using YDock.View;

namespace YDock.Model
{
    public class LayoutGroup : BaseLayoutGroup
    {
        public LayoutGroup(DockSide side, DockManager dockManager)
        {
            _side = side;
            _dockManager = dockManager;
        }

        protected override void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnChildrenPropertyChanged(sender, e);
            if (e.PropertyName == "IsActive")
                IsActive = (sender as DockElement).IsActive;
        }

        private DockManager _dockManager;
        public override DockManager DockManager
        {
            get
            {
                return _dockManager;
            }
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }

        public override void Dispose()
        {
            _dockManager = null;
        }
    }

    public class LayoutDocumentGroup : LayoutGroup
    {
        public LayoutDocumentGroup(DockManager dockManager) : base(DockSide.None, dockManager)
        {

        }

        public IEnumerable<IDockElement> ChildrenSorted
        {
            get
            {
                var listSorted = Children_CanSelect.ToList();
                listSorted.Sort();
                return listSorted;
            }
        }

        public void ActiveElement(IDockElement element)
        {
            (View as BaseGroupControl).SelectedIndex = Children.IndexOf(element);
            DockManager.ActiveElement = element as DockElement;
        }
    }
}