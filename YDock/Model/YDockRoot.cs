using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using YDock.Enum;
using YDock.Interface;

namespace YDock.Model
{
    public class YDockRoot : DependencyObject, INotifyPropertyChanged, IDockModel
    {
        public YDockRoot()
        {
            _InitSide();
        }

        private void _InitSide()
        {
            LeftSide = new YDockSide();
            RightSide = new YDockSide();
            TopSide = new YDockSide();
            BottomSide = new YDockSide();
            LeftSide.Children.CollectionChanged += OnSideChildrenChanged;
            RightSide.Children.CollectionChanged += OnSideChildrenChanged;
            TopSide.Children.CollectionChanged += OnSideChildrenChanged;
            BottomSide.Children.CollectionChanged += OnSideChildrenChanged;
        }

        private void OnSideChildrenChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IList<ILayoutElement> children = default(IList<ILayoutElement>);
            switch ((sender as YDockSide).Side)
            {
                case DockSide.Left:
                    children = _dockManager.LeftChildren as IList<ILayoutElement>;
                    break;
                case DockSide.Right:
                    children = _dockManager.RightChildren as IList<ILayoutElement>;
                    break;
                case DockSide.Top:
                    children = _dockManager.TopChildren as IList<ILayoutElement>;
                    break;
                case DockSide.Bottom:
                    children = _dockManager.BottomChildren as IList<ILayoutElement>;
                    break;
            }

            if (e.OldItems?.Count > 0)
                foreach (ILayoutElement item in e.OldItems)
                    children.Remove(item);

            if (e.NewItems?.Count > 0)
                foreach (ILayoutElement item in e.NewItems)
                    children.Add(item);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Parent
        private DockManager _dockManager;
        public DockManager DockManager
        {
            get { return _dockManager; }
            set { _dockManager = value; }
        }

        #endregion


        #region DockSide

        private YDockSide _leftSide;
        public YDockSide LeftSide
        {
            get { return _leftSide; }
            set
            {
                if (_leftSide != value)
                {
                    _leftSide = value;
                    if (_leftSide != null)
                    {
                        _leftSide.Root = this;
                        _leftSide.Side = DockSide.Left;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("LeftSide"));
                }
            }
        }

        private YDockSide _rightSide;
        public YDockSide RightSide
        {
            get { return _rightSide; }
            set
            {
                if (_rightSide != value)
                {
                    _rightSide = value;
                    if (_rightSide != null)
                    {
                        _rightSide.Root = this;
                        _rightSide.Side = DockSide.Right;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("RightSide"));
                }
            }
        }

        private YDockSide _topSide;
        public YDockSide TopSide
        {
            get { return _topSide; }
            set
            {
                if (_topSide != value)
                {
                    _topSide = value;
                    if (_topSide != null)
                    {
                        _topSide.Root = this;
                        _topSide.Side = DockSide.Top;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("TopSide"));
                }
            }
        }

        private YDockSide _bottomSide;
        public YDockSide BottomSide
        {
            get { return _bottomSide; }
            set
            {
                if (_bottomSide != value)
                {
                    _bottomSide = value;
                    if (_bottomSide != null)
                    {
                        _bottomSide.Root = this;
                        _bottomSide.Side = DockSide.Bottom;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("BottomSide"));
                }
            }
        }

        private IDockView _view;
        public IDockView View
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

        public DockSide Side
        {
            get
            {
                return DockSide.None;
            }
        }

        #endregion
    }
}