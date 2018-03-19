using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
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
        private UIElement _content;
        public UIElement Content
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
            internal set { SetValue(TitleProperty, value); }
            get { return (string)GetValue(TitleProperty); }
        }
        #endregion

        #region ImageSource
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(DockElement));

        public ImageSource ImageSource
        {
            internal set { SetValue(ImageSourceProperty, value); }
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
        private bool isVisible = false;
        /// <summary>
        /// Content是否可见
        /// </summary>
        public bool IsVisible
        {
            internal set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsVisible"));
                }
            }
            get { return isVisible; }
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
                }
            }
            get { return _canSelect; }
        }
        #endregion

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

        /// <summary>
        /// 用于保存布局信息的Level
        /// </summary>
        internal int Level
        {
            get
            {
                UpdateLevel();
                return _level;
            }
        }
        private int _level = -1;

        internal void UpdateLevel()
        {
            _level = -1;
            //Float与DockBar模式不计算
            if (_container == null || Mode != DockMode.Normal)
                return;
            else
            {
                var parent = _container.View;
                while (true)
                {
                    _level++;
                    if (parent.DockViewParent is LayoutRootPanel)
                        break;
                    parent = parent.DockViewParent;
                }
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
                return _container == null ? false : Mode != DockMode.Float;
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
                return _container == null ? false : Mode == DockMode.Normal;
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

        public void ToFloat()
        {
            if (!CanFloat) return;
            if (_container != null)
            {
                if (Mode == DockMode.Normal)
                    UpdateLevel();

                //注意切换模式
                Mode = DockMode.Float;
                var dockManager = DockManager;
                _container.Detach(this);
                _container = null;

                var group = new LayoutGroup(Side, Mode, dockManager);
                group.Attach(this);
                var groupctrl = new AnchorSideGroupControl(group) { DesiredHeight = DesiredHeight, DesiredWidth = DesiredWidth };
                var wnd = new AnchorGroupWindow(dockManager)
                {
                    Height = DesiredHeight,
                    Width = DesiredWidth,
                    Left = FloatLeft,
                    Top = FloatTop
                };
                wnd.AttachChild(groupctrl, AttachMode.None, 0);
                wnd.Show();
            }
        }

        public void ToDock()
        {
            if (!CanDock) return;
            if (_container != null)
            {
                //默认向下停靠
                if (Side == DockSide.None)
                    Side = DockSide.Bottom;

                Mode = DockMode.Normal;

                var dockManager = DockManager;
                _container.Detach(this);
                _container = null;

                if (_level < 0)
                    _ToRoot(dockManager);
                else
                {
                    IDockView view = dockManager.LayoutRootPanel.FindChildByLevel(_level, Side);
                    if (view != null)
                    {
                        if (view is BaseGroupControl)
                        {
                            var group = (view as BaseGroupControl).Model as ILayoutGroup;
                            group.Attach(this);
                        }
                        if (view is LayoutGroupPanel)
                        {
                            var panal = (view as LayoutGroupPanel);
                            var group = new LayoutGroup(Side, Mode, dockManager);
                            group.Attach(this);
                            var groupctrl = new AnchorSideGroupControl(group) { DesiredHeight = DesiredHeight, DesiredWidth = DesiredWidth };
                            switch (Side)
                            {
                                case DockSide.Left:
                                    panal.AttachChild(groupctrl, AttachMode.Left, 0);
                                    break;
                                case DockSide.Right:
                                    panal.AttachChild(groupctrl, AttachMode.Right, panal.Count);
                                    break;
                                case DockSide.Top:
                                    panal.AttachChild(groupctrl, AttachMode.Top, 0);
                                    break;
                                case DockSide.Bottom:
                                    panal.AttachChild(groupctrl, AttachMode.Bottom, panal.Count);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void ToDockAsDocument()
        {
            if (!CanDockAsDocument) return;
            if (_container != null)
            {
                var dockManager = DockManager;
                _container.Detach(this);
                _container = null;
                Side = DockSide.None;
                Mode = DockMode.Normal;

                dockManager.Root.DocumentModel.Attach(this, 0);
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
                    UpdateLevel();
                    Mode = DockMode.DockBar;
                    switch (Side)
                    {
                        case DockSide.Left:
                            dockManager.Root.LeftSide.Attach(this);
                            break;
                        case DockSide.Right:
                            dockManager.Root.RightSide.Attach(this);
                            break;
                        case DockSide.Top:
                            dockManager.Root.TopSide.Attach(this);
                            break;
                        case DockSide.Bottom:
                            dockManager.Root.BottomSide.Attach(this);
                            break;
                    }
                }
                else if (Mode == DockMode.DockBar)
                    _ToRoot(dockManager);
            }
        }

        public void Hide()
        {
            if (!CanHide) return;
            if (DockManager.ActiveElement == this)
                DockManager.ActiveElement = null;
            if (DockManager.AutoHideElement == this)
                DockManager.AutoHideElement = null;
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

        #region Dispose
        private bool _isDisposed = false;
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        public void Dispose()
        {
            if (_isDisposed) return;
            _dockControl = null;
            _content = null;
            _container = null;
            _isDisposed = true;
        }
        #endregion
    }
}