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
    public class DockSideModel : BaseLayoutGroup
    {
        public DockSideModel()
        {
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

        public override void Dispose()
        {
            base.Dispose();
            _root = null;
        }
    }
}