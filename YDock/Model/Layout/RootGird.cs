using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDock.Interface;

namespace YDock.Model
{
    public class RootGird : IModel
    {
        public RootGird()
        {

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
        private IView _view;
        public IView View
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
        #endregion
    }
}