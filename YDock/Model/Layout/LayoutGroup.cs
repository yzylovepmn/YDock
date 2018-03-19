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
using YDock.View;

namespace YDock.Model
{
    public class LayoutGroup : BaseLayoutGroup
    {
        public LayoutGroup(DockSide side, DockMode mode, DockManager dockManager)
        {
            _side = side;
            _mode = mode;
            _dockManager = dockManager;
        }

        protected override void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnChildrenCollectionChanged(sender, e);
            if (_view == null) return;
            if (e.NewItems?.Count > 0)
                (_view as TabControl).SelectedIndex = IndexOf(e.NewItems[e.NewItems.Count - 1] as IDockElement);
            else (_view as TabControl).SelectedIndex = Children_CanSelect.Count() - 1;
        }

        protected override void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnChildrenPropertyChanged(sender, e);
            if (e.PropertyName == "CanSelect")
            {
                if (_view == null) return;
                if ((sender as DockElement).CanSelect)
                    (_view as TabControl).SelectedIndex = Children_CanSelect.Count() - 1;
                else (_view as TabControl).SelectedIndex = Children_CanSelect.Count() > 0 ? 0 : -1;
                if (Children_CanSelect.Count() == 0)
                    _DetachFromParent();
            }
        }

        private DockManager _dockManager;
        public override DockManager DockManager
        {
            get
            {
                return _dockManager;
            }
        }

        public override void SetActive(IDockElement element)
        {
            base.SetActive(element);
            if (_view != null)
                (_view as TabControl).SelectedIndex = IndexOf(element);
            else//_view不存在则要创建新的_view
            {

            }
        }

        public override void Detach(IDockElement element)
        {
            base.Detach(element);
            //保存Size信息
            if (_view != null)
            {
                (element as DockElement).DesiredHeight = (_view as BaseGroupControl).ActualHeight;
                (element as DockElement).DesiredWidth = (_view as BaseGroupControl).ActualWidth;
                //如果Children_CanSelect数量为0，且Container不是LayoutDocumentGroup，则尝试将view从界面移除
                if (Children_CanSelect.Count() == 0) //如果Children_CanSelect数量为0
                    _DetachFromParent();
            }
        }

        public override void Attach(IDockElement element, int index = -1)
        {
            if (!element.Side.Assert())
                throw new ArgumentException("Side is illegal!");
            base.Attach(element, index);
        }

        private void _DetachFromParent()
        {
            var ret = (_view as ILayoutGroupControl).TryDeatchFromParent();
            if (ret)
            {
                _view = null;
                if (_children.Count == 0)
                    Dispose();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_view != null)
                _dockManager.DragManager.OnDragStatusChanged -= (_view as BaseGroupControl).OnDragStatusChanged;
            _dockManager = null;
        }
    }

    public class LayoutDocumentGroup : LayoutGroup
    {
        public LayoutDocumentGroup(DockMode mode, DockManager dockManager) : base(DockSide.None, mode, dockManager)
        {

        }

        protected override void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnChildrenPropertyChanged(sender, e);
            if (e.PropertyName == "IsActive")
                IsActive = (sender as IDockElement).IsActive;
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

        public IEnumerable<IDockElement> ChildrenSorted
        {
            get
            {
                var listSorted = Children_CanSelect.ToList();
                listSorted.Sort();
                return listSorted;
            }
        }

        public override void Attach(IDockElement element, int index = -1)
        {
            if (element.Side != DockSide.None)
                throw new ArgumentException("Side is illegal!");
            base.Attach(element, index);
            if (element.IsActive) IsActive = true;
        }

        public override void Detach(IDockElement element)
        {
            base.Detach(element);
            if (element.IsActive) IsActive = false;
            if (Children_CanSelect.Count() == 0 && _view?.DockViewParent != null)
            {
                var ctrl = _view as LayoutDocumentGroupControl;
                var panel = _view.DockViewParent as LayoutGroupDocumentPanel;
                if (panel.Children.Count > 1)
                {
                    panel.DetachChild(_view);
                    if (DockManager.Root.DocumentModel == this)
                    {
                        var child = panel.Children[0] as LayoutDocumentGroupControl;
                        DockManager.Root.DocumentModel = child.Model as BaseLayoutGroup;
                    }
                }
            }
        }
    }
}