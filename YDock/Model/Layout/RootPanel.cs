using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using YDock.Interface;

namespace YDock.Model
{
    [ContentProperty("Children")]
    public class RootPanel : IModel
    {
        public RootPanel()
        {
            _tab = new DocumentTab(this);
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

        #region View
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

        private DocumentTab _tab;
        public DocumentTab Tab
        {
            get { return _tab; }
        }

        public ObservableCollection<ILayoutElement> Children
        {
            get { return _tab.Children; }
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