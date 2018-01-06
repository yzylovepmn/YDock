using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using YDock.Enum;

namespace YDock.Model
{
    [ContentProperty("RootPanel")]
    public class YDockRoot : DependencyObject, INotifyPropertyChanged
    {
        public YDockRoot()
        {
            RootPanel = new RootPanel();
            LeftSide = new YDockSide();
            RightSide = new YDockSide();
            TopSide = new YDockSide();
            BottomSide = new YDockSide();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Parent
        private YDock _dockManager;
        public YDock DockManager
        {
            get { return _dockManager; }
            set { _dockManager = value; }
        }

        #endregion

        #region RootPanel

        private RootPanel _rootPanel;
        public RootPanel RootPanel
        {
            get { return _rootPanel; }
            set
            {
                if (_rootPanel != value)
                {
                    _rootPanel = value;
                    if (_rootPanel != null)
                        _rootPanel.Root = this;
                    PropertyChanged(this, new PropertyChangedEventArgs("RootGrid"));
                }
            }
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

        #endregion
    }
}