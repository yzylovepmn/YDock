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
    public class DockSideGroup : BaseLayoutGroup
    {
        public DockSideGroup()
        {
            _mode = DockMode.DockBar;
        }

        #region Root
        private DockRoot _root;
        public DockRoot Root
        {
            get { return _root; }
            set
            {
                if (_root != value)
                    _root = value;
            }
        }
        #endregion


        public override DockManager DockManager
        {
            get
            {
                return _root.DockManager;
            }
        }

        public override void SetActive(IDockElement element)
        {
            DockManager.AutoHideElement = element;
            base.SetActive(element);
        }

        public override void Detach(IDockElement element)
        {
            base.Detach(element);
            if (DockManager.AutoHideElement == element)
                DockManager.AutoHideElement = null;
        }

        public override void ToFloat()
        {
            foreach (var child in _children.ToList())
                child.ToFloat();
        }

        public override void Dispose()
        {
            base.Dispose();
            _root = null;
        }
    }
}