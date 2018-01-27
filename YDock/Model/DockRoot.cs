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
    public class DockRoot : DependencyObject, INotifyPropertyChanged, IDockModel
    {
        public DockRoot()
        {
            _InitSide();
        }

        private void _InitSide()
        {
            LeftSide = new DockSideModel();
            RightSide = new DockSideModel();
            TopSide = new DockSideModel();
            BottomSide = new DockSideModel();
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

        private DockSideModel _leftSide;
        public DockSideModel LeftSide
        {
            get { return _leftSide; }
            set
            {
                if (_leftSide != value)
                {
                    if (_leftSide != null)
                        _leftSide.Dispose();
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

        private DockSideModel _rightSide;
        public DockSideModel RightSide
        {
            get { return _rightSide; }
            set
            {
                if (_rightSide != value)
                {
                    if (_rightSide != null)
                        _rightSide.Dispose();
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

        private DockSideModel _topSide;
        public DockSideModel TopSide
        {
            get { return _topSide; }
            set
            {
                if (_topSide != value)
                {
                    if (_topSide != null)
                        _topSide.Dispose();
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

        private DockSideModel _bottomSide;
        public DockSideModel BottomSide
        {
            get { return _bottomSide; }
            set
            {
                if (_bottomSide != value)
                {
                    if (_bottomSide != null)
                        _bottomSide.Dispose();
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
            internal set
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

        private BaseLayoutGroup _documentModel;
        internal BaseLayoutGroup DocumentModel
        {
            get { return _documentModel; }
            set
            {
                if (_documentModel != value)
                    _documentModel = value;
            }
        }

        internal void AddDocument(IDockElement ele)
        {
            _documentModel.Children.Add(ele);
        }

        internal void AddSideChild(IDockElement ele, DockSide side)
        {
            switch (side)
            {
                case DockSide.Left:
                    LeftSide.Children.Add(ele);
                    break;
                case DockSide.Right:
                    RightSide.Children.Add(ele);
                    break;
                case DockSide.Top:
                    TopSide.Children.Add(ele);
                    break;
                case DockSide.Bottom:
                    BottomSide.Children.Add(ele);
                    break;
            }
        }

        public void Dispose()
        {
            LeftSide = null;
            RightSide = null;
            TopSide = null;
            BottomSide = null;
            _dockManager = null;
            PropertyChanged = null;
        }

        #endregion
    }
}