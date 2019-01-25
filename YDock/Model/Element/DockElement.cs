using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Xml.Linq;
using YDock.Enum;
using YDock.Interface;
using YDock.View;

namespace YDock.Model
{
    public class DockElement : DependencyObject, IDockElement, IComparable<DockElement>
    {
        internal DockElement(bool isDocument = false)
        {
            _isDocument = isDocument;
        }

        #region Ctrl
        private DockControl _dockControl;
        public DockControl DockControl
        {
            get { return _dockControl; }
            internal set
            {
                if (_dockControl != value)
                    _dockControl = value;
            }
        }
        #endregion

        #region Content
        private object _content;
        public object Content
        {
            get { return _content; }
            internal set
            {
                if (_content != value)
                {
                    _content = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Content"));
                }
            }
        }
        #endregion

        #region Title
        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register("Title", typeof(string), typeof(DockElement),
                new FrameworkPropertyMetadata(string.Empty));

        public string Title
        {
            set { SetValue(TitleProperty, value); }
            get { return (string)GetValue(TitleProperty); }
        }
        #endregion

        #region ImageSource
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(DockElement));

        public ImageSource ImageSource
        {
            set { SetValue(ImageSourceProperty, value); }
            get { return (ImageSource)GetValue(ImageSourceProperty); }
        }
        #endregion

        #region DockSide
        public static readonly DependencyProperty SideProperty =
            DependencyProperty.Register("Side", typeof(DockSide), typeof(DockElement));

        public DockSide Side
        {
            internal set { SetValue(SideProperty, value); }
            get { return (DockSide)GetValue(SideProperty); }
        }
        #endregion

        #region ID
        private int _id;
        public int ID
        {
            get { return _id; }
            internal set
            {
                if (_id != value)
                    _id = value;
            }
        }
        #endregion

        #region ToolTip
        public string ToolTip
        {
            get
            {
                if (Content is IDockDocSource)
                    return (Content as IDockDocSource).FullFileName;
                return Title;
            }
        }
        #endregion

        #region DockStatus
        private DockMode _mode;
        public DockMode Mode
        {
            get
            {
                return _mode;
            }
            internal set
            {
                if (_mode != value)
                {
                    _mode = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Mode"));
                }
            }
        }
        #endregion

        #region IsVisible
        private bool _isVisible = false;
        /// <summary>
        /// Content是否可见
        /// </summary>
        public bool IsVisible
        {
            internal set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsVisible"));
                }
            }
            get { return _isVisible; }
        }
        #endregion

        #region IsDocument
        private bool _isDocument;
        /// <summary>
        /// 是否以Document模式注册，该属性将影响Dock的浮动窗口的模式
        /// </summary>
        public bool IsDocument { get { return _isDocument; } }
        #endregion

        #region IsActive
        private bool _isActive = false;
        /// <summary>
        /// 是否为当前的活动窗口
        /// </summary>
        public bool IsActive
        {
            internal set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsActive"));
                }
            }
            get { return _isActive; }
        }
        #endregion

        #region CanSelect
        private bool _canSelect = false;
        /// <summary>
        /// 是否显示在用户界面供用户点击显示，默认为false
        /// </summary>
        public bool CanSelect
        {
            internal set
            {
                if (_canSelect != value)
                {
                    _canSelect = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CanSelect"));
                    if (_canSelect && _isDocument)
                        DockManager.PushBackwards(_id);
                }
            }
            get { return _canSelect; }
        }
        #endregion

        public bool IsDocked => CanSelect && _container is LayoutGroup && Mode == DockMode.Normal;

        public bool IsFloat => CanSelect && _container is LayoutGroup && Mode == DockMode.Float;

        public bool IsAutoHide => _container != null && this == DockManager.AutoHideElement;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ILayoutGroup _container;
        public ILayoutGroup Container
        {
            get
            {
                return _container;
            }
            internal set
            {
                if (_container != value)
                    _container = value;
            }
        }

        public DockManager DockManager
        {
            get
            {
                return _container?.DockManager;
            }
        }


        #region Size
        private double _desiredWidth;
        public double DesiredWidth
        {
            get
            {
                return _desiredWidth;
            }
            set
            {
                _desiredWidth = value;
            }
        }

        private double _desiredHeight;
        public double DesiredHeight
        {
            get
            {
                return _desiredHeight;
            }
            set
            {
                _desiredHeight = value;
            }
        }

        private double _floatLeft;
        public double FloatLeft
        {
            get { return _floatLeft; }
            set
            {
                if (_floatLeft != value)
                    _floatLeft = value;
            }
        }

        private double _floatTop;
        public double FloatTop
        {
            get { return _floatTop; }
            set
            {
                if (_floatTop != value)
                    _floatTop = value;
            }
        }
        #endregion

        #region Interface
        public bool CanFloat
        {
            get
            {
                return _container == null ? false : (Mode != DockMode.Float || _container?.View == null || _container.Children.Count() > 1 || _container.View.DockViewParent != null || !_canSelect);
            }
        }

        public bool CanDock
        {
            get
            {
                return _container == null ? false : Mode != DockMode.Normal;
            }
        }

        public bool CanDockAsDocument
        {
            get
            {
                return true;
            }
        }

        public bool CanSwitchAutoHideStatus
        {
            get
            {
                return _container == null ? false : Mode != DockMode.Float;
            }
        }

        public bool CanHide
        {
            get
            {
                return true;
            }
        }

        public int CompareTo(DockElement other)
        {
            return Title.CompareTo(other.Title);
        }

        public void ToFloat(bool isActive = true)
        {
            if (!CanFloat && isActive)
            {
                _dockControl.SetActive();
                return;
            }
            if (_container != null)
            {
                CanSelect = true;
                //注意切换模式
                Mode = DockMode.Float;
                var dockManager = DockManager;
                _container.Detach(this);
                _container = null;
                BaseFloatWindow wnd;
                BaseGroupControl groupctrl;
                if (!IsDocument)
                {
                    var group = new LayoutGroup(Side, Mode, dockManager);
                    group.Attach(this);
                    groupctrl = new AnchorSideGroupControl(group) { DesiredHeight = DesiredHeight, DesiredWidth = DesiredWidth };
                    wnd = new AnchorGroupWindow(dockManager)
                    {
                        Height = DesiredHeight,
                        Width = DesiredWidth,
                        Left = FloatLeft,
                        Top = FloatTop
                    };
                }
                else
                {
                    var group = new LayoutDocumentGroup(Mode, dockManager);
                    group.Attach(this);
                    groupctrl = new LayoutDocumentGroupControl(group) { DesiredHeight = DesiredHeight, DesiredWidth = DesiredWidth };
                    wnd = new DocumentGroupWindow(dockManager)
                    {
                        Height = DesiredHeight,
                        Width = DesiredWidth,
                        Left = FloatLeft,
                        Top = FloatTop
                    };
                }
                wnd.AttachChild(groupctrl, AttachMode.None, 0);
                wnd.Show();

                if (isActive)
                    _dockControl.SetActive();
            }
        }

        public void ToDock(bool isActive = true)
        {
            if (!CanDock && isActive)
            {
                _dockControl.SetActive();
                return;
            }
            if (_container != null)
            {
                CanSelect = true;
                Mode = DockMode.Normal;
                var dockManager = DockManager;
                var group = _container as LayoutGroup;
                if (group == null || group.AttachObj == null || !group.AttachObj.AttachTo())
                {
                    //默认向下停靠
                    if (Side == DockSide.None)
                        Side = DockSide.Bottom;
                    _container?.Detach(this);
                    _container = null;
                    _ToRoot(dockManager);
                }
                if (isActive)
                    _dockControl.SetActive();
            }
        }

        public void ToDockAsDocument(bool isActive = true)
        {
            if (!CanDockAsDocument && isActive)
            {
                _dockControl.SetActive();
                return;
            }
            if (_container != null)
            {
                CanSelect = true;
                var dockManager = DockManager;
                _container.Detach(this);
                _container = null;
                Side = DockSide.None;
                Mode = DockMode.Normal;

                dockManager.Root.DocumentModels[0].Attach(this, 0);

                if (isActive)
                    _dockControl.SetActive();
            }
        }

        public void SwitchAutoHideStatus()
        {
            if (!CanSwitchAutoHideStatus) return;
            if (_container != null)
            {
                var dockManager = DockManager;
                _container.Detach(this);
                _container = null;

                if (Mode == DockMode.Normal)
                {
                    Mode = DockMode.DockBar;
                    dockManager.Root.AddSideChild(this, Side);
                }
                else if (Mode == DockMode.DockBar)
                    _ToRoot(dockManager);
            }
        }

        public void ToDockSide(DockSide side, bool isActive = false)
        {
            if (side != DockSide.Left && side != DockSide.Top && side != DockSide.Right && side != DockSide.Bottom) return;
            if (_container != null)
            {
                if (side != Side || Mode != DockMode.DockBar)
                {
                    var dockManager = DockManager;
                    _container.Detach(this);
                    _container = null;

                    Mode = DockMode.DockBar;
                    Side = side;
                    dockManager.Root.AddSideChild(this, Side);
                }

                if (isActive)
                    _dockControl.SetActive();
            }
        }

        public void Hide()
        {
            if (!CanHide) return;
            if (_isVisible && _isDocument)
            {
                IsVisible = false;
                var id = DockManager.FindVisibleCtrl();
                DockManager.PushBackwards(id);
                DockManager.ShowByID(id);
            }
            if (DockManager.AutoHideElement == this)
                DockManager.AutoHideElement = null;
            _dockControl.SetActive(false);
            CanSelect = false;
        }

        private void _ToRoot(DockManager dockManager)
        {
            Mode = DockMode.Normal;
            var group = new LayoutGroup(Side, Mode, dockManager);
            group.Attach(this);
            var groupctrl = new AnchorSideGroupControl(group) { DesiredHeight = DesiredHeight, DesiredWidth = DesiredWidth };
            switch (Side)
            {
                case DockSide.Left:
                    dockManager.LayoutRootPanel.RootGroupPanel.AttachChild(groupctrl, AttachMode.Left, 0);
                    break;
                case DockSide.Right:
                    dockManager.LayoutRootPanel.RootGroupPanel.AttachChild(groupctrl, AttachMode.Right, dockManager.LayoutRootPanel.RootGroupPanel.Count);
                    break;
                case DockSide.Top:
                    dockManager.LayoutRootPanel.RootGroupPanel.AttachChild(groupctrl, AttachMode.Top, 0);
                    break;
                case DockSide.Bottom:
                    dockManager.LayoutRootPanel.RootGroupPanel.AttachChild(groupctrl, AttachMode.Bottom, dockManager.LayoutRootPanel.RootGroupPanel.Count);
                    break;
            }
        }
        #endregion

        #region Save & Load
        public XElement Save()
        {
            var ele = new XElement("Item");
            ele.SetAttributeValue("ID", _id);
            ele.SetAttributeValue("DesiredWidth", _desiredWidth);
            ele.SetAttributeValue("DesiredHeight", _desiredHeight);
            ele.SetAttributeValue("FloatLeft", _floatLeft);
            ele.SetAttributeValue("FloatTop", _floatTop);
            ele.SetAttributeValue("CanSelect", _canSelect);
            return ele;
        }

        public void Load(XElement ele)
        {
            _desiredWidth = double.Parse(ele.Attribute("DesiredWidth").Value);
            _desiredHeight = double.Parse(ele.Attribute("DesiredHeight").Value);
            _floatLeft = double.Parse(ele.Attribute("FloatLeft").Value);
            _floatTop = double.Parse(ele.Attribute("FloatTop").Value);
            CanSelect = bool.Parse(ele.Attribute("CanSelect").Value);
        }
        #endregion

        #region Dispose
        private bool _isDisposed = false;
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _container?.Detach(this);
            _dockControl = null;
            if (_content is IDockSource)
                (_content as IDockSource).DockControl = null;
            _content = null;
            _container = null;
            _isDisposed = true;
        }
        #endregion
    }
}