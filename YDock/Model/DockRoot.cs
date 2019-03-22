using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;
using YDock.Enum;
using YDock.Interface;

namespace YDock.Model
{
    public class DockRoot : DependencyObject, INotifyPropertyChanged, IDockModel
    {
        public DockRoot() { }

        private void _InitSide()
        {
            LeftSide = new DockSideGroup();
            RightSide = new DockSideGroup();
            TopSide = new DockSideGroup();
            BottomSide = new DockSideGroup();
            _documentModels = new List<BaseLayoutGroup>();
            _documentModels.Add(new LayoutDocumentGroup(DockMode.Normal, _dockManager));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Parent
        private DockManager _dockManager;
        public DockManager DockManager
        {
            get { return _dockManager; }
            set
            {
                _dockManager = value;
                if (_dockManager != null)
                    _InitSide();
            }
        }
        #endregion


        #region DockSide

        private DockSideGroup _leftSide;
        public DockSideGroup LeftSide
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

        private DockSideGroup _rightSide;
        public DockSideGroup RightSide
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

        private DockSideGroup _topSide;
        public DockSideGroup TopSide
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

        private DockSideGroup _bottomSide;
        public DockSideGroup BottomSide
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

        internal List<BaseLayoutGroup> DocumentModels
        {
            get { return _documentModels; }
            set
            {
                if (_documentModels != value)
                    _documentModels = value;
            }
        }
        private List<BaseLayoutGroup> _documentModels;

        internal void AddSideChild(IDockElement ele, DockSide side)
        {
            switch (side)
            {
                case DockSide.Left:
                    LeftSide.Attach(ele);
                    break;
                case DockSide.Right:
                    RightSide.Attach(ele);
                    break;
                case DockSide.Top:
                    TopSide.Attach(ele);
                    break;
                case DockSide.Bottom:
                    BottomSide.Attach(ele);
                    break;
            }
        }

        public XElement GenerateLayout()
        {
            var ele = new XElement("ToolBar");

            // Left bar
            var node = new XElement("LeftBar");
            foreach (var item in LeftSide.Children)
                node.Add(new XElement("Item", item.ID));
            ele.Add(node);

            // Top bar
            node = new XElement("TopBar");
            foreach (var item in TopSide.Children)
                node.Add(new XElement("Item", item.ID));
            ele.Add(node);

            // Right bar
            node = new XElement("RightBar");
            foreach (var item in RightSide.Children)
                node.Add(new XElement("Item", item.ID));
            ele.Add(node);

            // Bottom bar
            node = new XElement("BottomBar");
            foreach (var item in BottomSide.Children)
                node.Add(new XElement("Item", item.ID));
            ele.Add(node);

            return ele;
        }

        public void LoadLayout(XElement root)
        {
            foreach (var item in root.Element("LeftBar").Elements())
            {
                var id = int.Parse(item.Value);
                var ele = _dockManager.GetDockControl(id);
                ele?.ProtoType.ToDockSide(DockSide.Left);
            }

            foreach (var item in root.Element("TopBar").Elements())
            {
                var id = int.Parse(item.Value);
                var ele = _dockManager.GetDockControl(id);
                ele?.ProtoType.ToDockSide(DockSide.Top);
            }

            foreach (var item in root.Element("RightBar").Elements())
            {
                var id = int.Parse(item.Value);
                var ele = _dockManager.GetDockControl(id);
                ele?.ProtoType.ToDockSide(DockSide.Right);
            }

            foreach (var item in root.Element("BottomBar").Elements())
            {
                var id = int.Parse(item.Value);
                var ele = _dockManager.GetDockControl(id);
                ele?.ProtoType.ToDockSide(DockSide.Bottom);
            }
        }

        public void Dispose()
        {
            _documentModels.Clear();
            _documentModels = null;
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