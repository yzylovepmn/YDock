using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;
using YDock.View;

namespace YDock
{
    [ContentProperty("Root")]
    public class DockManager : Control, IDockView
    {
        static DockManager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockManager), new FrameworkPropertyMetadata(typeof(DockManager)));
            FocusableProperty.OverrideMetadata(typeof(DockManager), new FrameworkPropertyMetadata(false));
        }

        public DockManager()
        {
            Root = new DockRoot();
            _dockControls = new List<IDockControl>();
            _floatControls = new List<IDockControl>();
        }

        #region Root
        private DockRoot _root;
        public DockRoot Root
        {
            get { return _root; }
            set
            {
                if (_root != value)
                {
                    if (_root != null)
                        _root.Dispose();
                    _root = value;
                    if (_root != null)
                        _root.DockManager = this;
                }
            }
        }
        #endregion


        #region DependencyProperty
        public static readonly DependencyProperty LeftSideProperty =
            DependencyProperty.Register("LeftSide", typeof(DockSideGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnLeftSideChanged)));

        public DockSideGroupControl LeftSide
        {
            get { return (DockSideGroupControl)GetValue(LeftSideProperty); }
            set { SetValue(LeftSideProperty, value); }
        }

        private static void OnLeftSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnLeftSideChanged(e);
        }

        protected virtual void OnLeftSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty RightSideProperty =
            DependencyProperty.Register("RightSide", typeof(DockSideGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnRightSideChanged)));

        public DockSideGroupControl RightSide
        {
            get { return (DockSideGroupControl)GetValue(RightSideProperty); }
            set { SetValue(RightSideProperty, value); }
        }

        private static void OnRightSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnRightSideChanged(e);
        }

        protected virtual void OnRightSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty TopSideProperty =
            DependencyProperty.Register("TopSide", typeof(DockSideGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnTopSideChanged)));

        public DockSideGroupControl TopSide
        {
            get { return (DockSideGroupControl)GetValue(TopSideProperty); }
            set { SetValue(TopSideProperty, value); }
        }

        private static void OnTopSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnTopSideChanged(e);
        }

        protected virtual void OnTopSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty BottomSideProperty =
            DependencyProperty.Register("BottomSide", typeof(DockSideGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnBottomSideChanged)));

        public DockSideGroupControl BottomSide
        {
            get { return (DockSideGroupControl)GetValue(BottomSideProperty); }
            set { SetValue(BottomSideProperty, value); }
        }

        private static void OnBottomSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnBottomSideChanged(e);
        }

        protected virtual void OnBottomSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty LayoutRootPanelProperty =
            DependencyProperty.Register("LayoutRootPanel", typeof(LayoutRootPanel), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnLayoutRootPanelChanged)));

        public LayoutRootPanel LayoutRootPanel
        {
            get { return (LayoutRootPanel)GetValue(LayoutRootPanelProperty); }
            set { SetValue(LayoutRootPanelProperty, value); }
        }

        private static void OnLayoutRootPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockManager)d).OnLayoutRootPanelChanged(e);
        }

        protected virtual void OnLayoutRootPanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }
        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_root.DockManager == this)
            {
                LayoutRootPanel = new LayoutRootPanel(_root);
                LeftSide = new DockSideGroupControl(_root.LeftSide);
                RightSide = new DockSideGroupControl(_root.RightSide);
                BottomSide = new DockSideGroupControl(_root.BottomSide);
                TopSide = new DockSideGroupControl(_root.TopSide);
            }
        }

        /// <summary>
        /// 自动隐藏窗口的Model
        /// </summary>
        internal IDockElement AutoHideElement
        {
            get { return LayoutRootPanel.AHWindow.Model; }
            set
            {
                if (LayoutRootPanel.AHWindow.Model != value)
                {
                    if(LayoutRootPanel.AHWindow.Model != null)
                        LayoutRootPanel.AHWindow.Model.IsVisible = false;
                    LayoutRootPanel.AHWindow.Model = value as DockElement;
                    if (LayoutRootPanel.AHWindow.Model != null)
                        LayoutRootPanel.AHWindow.Model.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// current ActiveElement
        /// </summary>
        internal DockElement ActiveElement
        {
            get { return _activeElement; }
            set
            {
                if (_activeElement != value)
                {
                    if (_activeElement != null)
                        _activeElement.IsActive = false;
                    _activeElement = value;
                    if (_activeElement != null)
                    {
                        _activeElement.IsActive = true;
                        AutoHideElement = null;
                    }
                }
            }
        }
        private DockElement _activeElement;

        public IDockModel Model
        {
            get
            {
                return null;
            }
        }

        public IDockView DockViewParent
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// all registed DockControl
        /// </summary>
        public IEnumerable<IDockControl> DockControls
        {
            get
            {
                foreach (var ctrl in _dockControls)
                    yield return ctrl;
            }
        }
        private IList<IDockControl> _dockControls;

        /// <summary>
        /// all registed FloatControls
        /// </summary>
        public IEnumerable<IDockControl> FloatControls
        {
            get
            {
                foreach (var ctrl in _floatControls)
                    yield return ctrl;
            }
        }
        private IList<IDockControl> _floatControls;

        public int AllControlsCount
        {
            get
            {
                return _dockControls.Count + _floatControls.Count;
            }
        }

        /// <summary>
        /// all Docked child
        /// </summary>
        internal IEnumerable<IDockElement> DockedChildren
        {
            get
            {
                foreach (DockControl ctrl in _dockControls)
                    yield return ctrl.Prototype;
            }
        }

        /// <summary>
        /// all Docked child
        /// </summary>
        internal IEnumerable<IDockElement> FloatedChildren
        {
            get
            {
                foreach (DockControl ctrl in _floatControls)
                    yield return ctrl.Prototype;
            }
        }

        #region Register
        /// <summary>
        /// 以选项卡模式向DockManager注册一个DockElement并返回对应的DockControl
        /// </summary>
        public DockControl RegisterDocument(string title, UIElement content, ImageSource imageSource = null)
        {
            DockElement ele = new DockElement()
            {
                ID = AllControlsCount,
                Title = title,
                Content = content,
                ImageSource = imageSource,
                Side = DockSide.None,
                Status = DockStatus.Docked
            };
            var ctrl = new DockControl(ele);
            _dockControls.Add(ctrl);
            _root.AddDocument(ele);
            return ctrl;
        }
        /// <summary>
        /// 以通用模式（必须制定停靠方向，否则默认停靠在左侧）向DockManager注册一个DockElement并返回对应的DockControl
        /// </summary>
        public DockControl RegisterDock(string title, UIElement content, ImageSource imageSource = null, DockSide side = DockSide.Left, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength)
        {
            DockElement ele = new DockElement()
            {
                ID = AllControlsCount,
                Title = title,
                Content = content,
                ImageSource = imageSource,
                Side = side,
                Status = DockStatus.AnchorSide,
                DesiredWidth = desiredWidth,
                DesiredHeight = desiredHeight
            };
            switch (side)
            {
                case DockSide.Left:
                case DockSide.Right:
                case DockSide.Top:
                case DockSide.Bottom:
                    _root.AddSideChild(ele, side);
                    break;
                default://其他非法方向返回NULL
                    ele.Dispose();
                    return null;
            }
            var ctrl = new DockControl(ele);
            _dockControls.Add(ctrl);
            return ctrl;
        }
        /// <summary>
        /// 以Float模式向DockManager注册一个DockElement并返回对应的DockControl
        /// </summary>
        public DockControl RegisterFloat(string title, UIElement content, ImageSource imageSource = null, DockSide side = DockSide.Left, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength)
        {
            DockElement ele = new DockElement()
            {
                ID = AllControlsCount,
                Title = title,
                Content = content,
                ImageSource = imageSource,
                Side = side,
                Status = DockStatus.Float,
                DesiredWidth = desiredWidth,
                DesiredHeight = desiredHeight
            };
            var ctrl = new DockControl(ele);
            _floatControls.Add(ctrl);
            return ctrl;
        }
        #endregion

        public void Dispose()
        {
            foreach (var ctrl in _dockControls)
                ctrl.Dispose();
            _dockControls.Clear();
            _dockControls = null;
            foreach (var ctrl in _floatControls)
                ctrl.Dispose();
            _floatControls.Clear();
            _floatControls = null;
            Root = null;
        }
    }
}