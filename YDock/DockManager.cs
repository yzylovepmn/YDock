using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;
using YDock.View;

namespace YDock
{
    [ContentProperty("Root")]
    public class DockManager : Control, IDockManager
    {
        static DockManager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockManager), new FrameworkPropertyMetadata(typeof(DockManager)));
            FocusableProperty.OverrideMetadata(typeof(DockManager), new FrameworkPropertyMetadata(false));
        }

        public DockManager()
        {
            Root = new DockRoot(this);
            _dragManager = new DragManager(this);
            _dockControls = new Dictionary<int, IDockControl>();
            _floatWindows = new List<BaseFloatWindow>();
            backwards = new Stack<int>();
            forwards = new Stack<int>();
        }

        #region Root
        private DockRoot _root;
        internal DockRoot Root
        {
            get { return _root; }
            set
            {
                if (_root != value)
                {
                    if (_root != null)
                        _root.Dispose();
                    _root = value;
                }
            }
        }
        #endregion

        #region Drag
        private DragManager _dragManager;
        internal DragManager DragManager
        {
            get { return _dragManager; }
        }

        public bool IsDragging { get { return _dragManager.IsDragging; } }
        #endregion

        #region MainWindow
        private Window _mainWindow;
        public Window MainWindow
        {
            get
            {
                if (_mainWindow == null)
                {
                    _mainWindow = Window.GetWindow(this);
                    _mainWindow.Closing += OnMainWindowClosing;
                }
                return _mainWindow;
            }
        }
        #endregion

        #region DependencyProperty

        #region DockImageSource
        public static readonly DependencyProperty DockImageSourceProperty =
            DependencyProperty.Register("DockImageSource", typeof(ImageSource), typeof(DockManager));

        /// <summary>
        /// 用于浮动窗口显示，一般用作应用程序的图标
        /// </summary>
        public ImageSource DockImageSource
        {
            set { SetValue(DockImageSourceProperty, value); }
            get { return (ImageSource)GetValue(DockImageSourceProperty); }
        }
        #endregion

        #region DockTitle
        public static readonly DependencyProperty DockTitleProperty =
            DependencyProperty.Register("DockTitle", typeof(string), typeof(DockManager),
                new FrameworkPropertyMetadata("YDock"));

        /// <summary>
        /// 用于浮动窗口显示，一般用作应用程序的Title
        /// </summary>
        public string DockTitle
        {
            set { SetValue(DockTitleProperty, value); }
            get { return (string)GetValue(DockTitleProperty); }
        }
        #endregion

        #region DocumentHeaderTemplate
        public static readonly DependencyProperty DocumentHeaderTemplateProperty =
            DependencyProperty.Register("DocumentHeaderTemplate", typeof(ControlTemplate), typeof(DockManager));

        public ControlTemplate DocumentHeaderTemplate
        {
            internal set { SetValue(DocumentHeaderTemplateProperty, value); }
            get { return (ControlTemplate)GetValue(DocumentHeaderTemplateProperty); }
        }
        #endregion

        #region Side & Root
        public static readonly DependencyProperty LeftSideProperty =
            DependencyProperty.Register("LeftSide", typeof(DockBarGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnLeftSideChanged)));

        public DockBarGroupControl LeftSide
        {
            get { return (DockBarGroupControl)GetValue(LeftSideProperty); }
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
            DependencyProperty.Register("RightSide", typeof(DockBarGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnRightSideChanged)));

        public DockBarGroupControl RightSide
        {
            get { return (DockBarGroupControl)GetValue(RightSideProperty); }
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
            DependencyProperty.Register("TopSide", typeof(DockBarGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnTopSideChanged)));

        public DockBarGroupControl TopSide
        {
            get { return (DockBarGroupControl)GetValue(TopSideProperty); }
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
            DependencyProperty.Register("BottomSide", typeof(DockBarGroupControl), typeof(DockManager),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnBottomSideChanged)));

        public DockBarGroupControl BottomSide
        {
            get { return (DockBarGroupControl)GetValue(BottomSideProperty); }
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

        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (_root.DockManager == this)
            {
                LayoutRootPanel = new LayoutRootPanel(_root);
                LeftSide = new DockBarGroupControl(_root.LeftSide);
                RightSide = new DockBarGroupControl(_root.RightSide);
                BottomSide = new DockBarGroupControl(_root.BottomSide);
                TopSide = new DockBarGroupControl(_root.TopSide);
            }
        }

        /// <summary>
        /// 自动隐藏窗口的Model
        /// </summary>
        internal IDockElement AutoHideElement
        {
            get { return LayoutRootPanel?.AHWindow.Model; }
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

        public event EventHandler ActiveDockChanged = delegate { };
        /// <summary>
        /// current ActiveElement
        /// </summary>
        internal IDockElement ActiveElement
        {
            get { return _activeElement; }
            set
            {
                if (_activeElement != value)
                {
                    var oldele = _activeElement;
                    _activeElement = value as DockElement;
                    if (oldele != null)
                        oldele.IsActive = false;
                    if (_activeElement != null)
                    {
                        _activeElement.IsActive = true;
                        //这里必须将AutoHideElement设为NULL，保证当前活动窗口只有一个
                        if (AutoHideElement != _activeElement)
                            AutoHideElement = null;

                        if (_activeElement.IsDocument)
                            PushBackwards(_activeElement.ID);
                    }
                    ActiveDockChanged(this, new EventArgs());

                    var newSelectedDocument = SelectedDocument;
                    if (_selectedDocument != newSelectedDocument)
                    {
                        _selectedDocument = newSelectedDocument;
                        SelectedDocumentChanged(this, new EventArgs());
                    }
                }
            }
        }
        private DockElement _activeElement;

        /// <summary>
        /// 当前活动的DockControl
        /// </summary>
        public IDockControl ActiveControl
        {
            get { return _activeElement?.DockControl; }
        }


        public event EventHandler SelectedDocumentChanged = delegate { };
        /// <summary>
        /// 当前选中的文档
        /// </summary>
        public IDockControl SelectedDocument
        {
            get
            {
                //优先返回活跃的文档
                if (ActiveControl != null && ActiveControl.IsDocument) return ActiveControl;
                if (_root == null) return null;
                var ele = (_root?.DocumentModels[0].View as TabControl).SelectedItem as DockElement;
                return ele?.DockControl;
            }
        }
        private IDockControl _selectedDocument;

        public event RoutedEventHandler DocumentToEmpty = delegate { };

        internal void RaiseDocumentToEmpty()
        {
            DocumentToEmpty(this, new RoutedEventArgs());
        }

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

        private List<BaseFloatWindow> _floatWindows;
        public IEnumerable<BaseFloatWindow> FloatWindows
        {
            get { return _floatWindows; }
        }

        internal void MoveFloatTo(BaseFloatWindow wnd, int index = 0)
        {
            _floatWindows.Remove(wnd);
            _floatWindows.Insert(index, wnd);
        }

        /// <summary>
        /// all registed DockControl
        /// </summary>
        public IEnumerable<IDockControl> DockControls
        {
            get
            {
                foreach (var ctrl in _dockControls)
                    yield return ctrl.Value;
            }
        }
        private Dictionary<int, IDockControl> _dockControls;

        internal void AddDockControl(IDockControl ctrl)
        {
            if (!_dockControls.ContainsKey(ctrl.ID))
                _dockControls.Add(ctrl.ID, ctrl);
        }

        internal void RemoveDockControl(IDockControl ctrl)
        {
            if (_dockControls.ContainsKey(ctrl.ID))
                _dockControls.Remove(ctrl.ID);
        }

        #region Register
        internal int id = 0;

        /// <summary>
        /// 以选项卡模式向DockManager注册一个DockElement
        /// </summary>
        /// <param name="title">标题栏文字</param>
        /// <param name="content">内容</param>
        /// <param name="imageSource">标题栏图标</param>
        /// <param name="canSelect">是否直接停靠在选项栏中供用户选择(默认为False)</param>
        /// <param name="desiredWidth">期望的宽度</param>
        /// <param name="desiredHeight">期望的高度</param>
        /// <returns></returns>
        public void RegisterDocument(IDockSource content, bool canSelect = false, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength)
        {
            DockElement ele = new DockElement(true)
            {
                ID = id++,
                Title = content.Header,
                Content = content as UIElement,
                ImageSource = content.Icon,
                Side = DockSide.None,
                Mode = DockMode.Normal,
                CanSelect = canSelect,
                DesiredWidth = desiredWidth,
                DesiredHeight = desiredHeight
            };
            var ctrl = new DockControl(ele);
            AddDockControl(ctrl);
            _root.DocumentModels[0].Attach(ele);
            content.DockControl = ctrl;
        }
        /// <summary>
        /// 以DockBar模式（必须指定停靠方向，否则默认停靠在左侧）向DockManager注册一个DockElement
        /// </summary>
        /// <param name="title">标题栏文字</param>
        /// <param name="content">内容</param>
        /// <param name="imageSource">标题栏图标</param>
        /// <param name="side">停靠方向（默认左侧）</param>
        /// <param name="canSelect">是否直接停靠在选项栏中供用户选择(默认为False)</param>
        /// <param name="desiredWidth">期望的宽度</param>
        /// <param name="desiredHeight">期望的高度</param>
        /// <returns></returns>
        public void RegisterDock(IDockSource content, DockSide side = DockSide.Left, bool canSelect = false, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength)
        {
            DockElement ele = new DockElement()
            {
                ID = id++,
                Title = content.Header,
                Content = content as UIElement,
                ImageSource = content.Icon,
                Side = side,
                Mode = DockMode.DockBar,
                CanSelect = canSelect,
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
                    break;
            }
            var ctrl = new DockControl(ele);
            AddDockControl(ctrl);
            content.DockControl = ctrl;
        }
        /// <summary>
        /// 以Float模式向DockManager注册一个DockElement
        /// </summary>
        /// <param name="title">标题栏文字</param>
        /// <param name="content">内容</param>
        /// <param name="imageSource">标题栏图标</param>
        /// <param name="side">停靠方向（默认左侧）</param>
        /// <param name="desiredWidth">期望的宽度</param>
        /// <param name="desiredHeight">期望的高度</param>
        /// <returns></returns>
        public void RegisterFloat(IDockSource content, DockSide side = DockSide.Left, double desiredWidth = Constants.DockDefaultWidthLength, double desiredHeight = Constants.DockDefaultHeightLength)
        {
            DockElement ele = new DockElement()
            {
                ID = id++,
                Title = content.Header,
                Content = content as UIElement,
                ImageSource = content.Icon,
                Side = side,
                Mode = DockMode.Float,
                DesiredWidth = desiredWidth,
                DesiredHeight = desiredHeight
            };
            var ctrl = new DockControl(ele);
            var group = new LayoutGroup(side, ele.Mode, this);
            group.Attach(ele);
            AddDockControl(ctrl);
            content.DockControl = ctrl;
        }
        #endregion

        public void ShowOrHide(IDockSource source)
        {
            if (source.DockControl.IsVisible)
                source.DockControl.Hide();
            else source.DockControl.Show();
        }

        internal static void ChangeSide(IDockView view, DockSide side)
        {
            if (view.Model != null && view.Model.Side == side) return;
            if (view is BaseGroupControl)
                (view.Model as BaseLayoutGroup).Side = side;
            if (view is LayoutGroupPanel)
                (view as LayoutGroupPanel).Side = side;
        }

        internal static void ClearAttachObj(IDockView view)
        {
            if (view is AnchorSideGroupControl)
                (view.Model as LayoutGroup).AttachObj?.Dispose();

            if (view is LayoutGroupPanel)
                foreach (var _view in (view as LayoutGroupPanel).Children.OfType<IDockView>())
                    ClearAttachObj(_view);
        }

        internal static void FormatChildSize(ILayoutSize child, Size size)
        {
            if (child == null) return;
            child.DesiredWidth = Math.Min(child.DesiredWidth, size.Width / 2);
            child.DesiredHeight = Math.Min(child.DesiredHeight, size.Height / 2);
        }

        internal static void ChangeDockMode(IDockView view, DockMode mode)
        {
            if (view is BaseGroupControl)
                (view.Model as BaseLayoutGroup).Mode = mode;
            if (view is LayoutGroupPanel)
                foreach (var _view in (view as LayoutGroupPanel).Children.OfType<IDockView>())
                    ChangeDockMode(_view, mode);
        }

        internal void AddFloatWindow(BaseFloatWindow window)
        {
            _floatWindows.Add(window);
        }

        internal void RemoveFloatWindow(BaseFloatWindow window)
        {
            if (_floatWindows.Contains(window))
                _floatWindows.Remove(window);
        }

        internal List<Window> _windows = new List<Window>();
        internal void UpdateWindowZOrder()
        {
            _windows.Clear();
            List<Window> unsorts = new List<Window>();
            foreach (Window wnd in Application.Current.Windows)
                if (wnd is BaseFloatWindow)
                    unsorts.Add(wnd);
            unsorts.Add(MainWindow);
            _windows.AddRange(SortWindowsTopToBottom(unsorts));
        }

        internal bool IsBehindToMainWindow(BaseFloatWindow wnd)
        {
            if (wnd is AnchorGroupWindow)
                return false;
            int index1 = _windows.IndexOf(_mainWindow);
            int index2 = _windows.IndexOf(wnd);
            return index2 > index1;
        }

        private IEnumerable<Window> SortWindowsTopToBottom(IEnumerable<Window> unsorted)
        {
            var byHandle = unsorted.ToDictionary(win =>
              ((new WindowInteropHelper(win)).Handle));

            for (IntPtr hWnd = Win32Helper.GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = Win32Helper.GetWindow(hWnd, Win32Helper.GW_HWNDNEXT))
                if (byHandle.ContainsKey(hWnd))
                    yield return byHandle[hWnd];
        }

        private void OnMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var fwnd in new List<BaseFloatWindow>(_floatWindows))
                fwnd.Close();
        }

        public void HideAll()
        {
            foreach (var wnd in new List<BaseFloatWindow>(_floatWindows))
                wnd.Close();
            _floatWindows.Clear();
            foreach (var dockControl in _dockControls.Values)
                dockControl.Hide();
        }

        public void UpdateTitleAll()
        {
            IDockSource source;
            foreach (var dockControl in _dockControls.Values)
            {
                source = dockControl.Content as IDockSource;
                if (source != null)
                    dockControl.Title = source.Header;
            }
        }

        #region Navigate
        public bool CanNavigateBackward
        {
            get { return backwards.Count > 1; }
        }

        public bool CanNavigateForward
        {
            get { return forwards.Count > 0; }
        }

        internal Stack<int> backwards;
        internal Stack<int> forwards;

        /// <summary>
        /// 向后导航
        /// </summary>
        public void NavigateBackward()
        {
            while (CanNavigateBackward)
            {
                forwards.Push(backwards.Pop());
                int id = backwards.Peek();
                var ctrl = _dockControls[id];
                if (ctrl != null)
                {
                    ctrl.ToDockAsDocument();
                    break;
                }
            }
        }

        /// <summary>
        /// 向前导航
        /// </summary>
        public void NavigateForward()
        {
            while (CanNavigateForward)
            {
                int id = forwards.Pop();
                backwards.Push(id);
                var ctrl = _dockControls[id];
                if (ctrl != null)
                {
                    ctrl.ToDockAsDocument();
                    break;
                }
            }
        }

        internal void PushBackwards(int id)
        {
            if (backwards.Count > 0 && backwards.Peek() == id) return;
            if (id < 0) return;
            backwards.Push(id);
            forwards.Clear();
        }

        public void ShowByID(int id)
        {
            if (_dockControls.ContainsKey(id))
                _dockControls[id].ToDockAsDocument();
        }

        internal int FindVisibleCtrl()
        {
            foreach (var id in backwards)
                if (_dockControls.ContainsKey(id)
                    && !_dockControls[id].IsActive
                    && _dockControls[id].CanSelect)
                    return id;
            return -1;
        }
        #endregion

        #region Attach
        /// <summary>
        /// attach source to target by <see cref="AttachMode"/>
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标</param>
        /// <param name="mode">附加模式</param>
        public void AttachTo(IDockControl source, IDockControl target, AttachMode mode, double ratio = 1)
        {
            if (target.Container.View == null) throw new InvalidOperationException("target must be visible!");
            if (target.IsDisposed) throw new InvalidOperationException("target is disposed!");
            if (source == target) throw new InvalidOperationException("source can not be target!");
            if (source == null || target == null) throw new ArgumentNullException("source or target is null!");
            if (source.Container.View != target.Container.View && source.CanSelect)
                source.SetActive();
            else if (source.Container != null)
            {
                //DockBar模式下无法合并，故先转换为Normal模式
                if (target.Mode == DockMode.DockBar)
                    target.ToDock();

                source.Container.Detach(source.ProtoType);


                double width = (target.Container.View as ILayoutViewWithSize).DesiredWidth
                    , height = (target.Container.View as ILayoutViewWithSize).DesiredHeight;

                if (mode == AttachMode.Right
                    || mode == AttachMode.Left
                    || mode == AttachMode.Left_WithSplit
                    || mode == AttachMode.Right_WithSplit)
                    width = (target.Container.View as ILayoutViewWithSize).DesiredWidth * ratio;

                if (mode == AttachMode.Top
                    || mode == AttachMode.Bottom
                    || mode == AttachMode.Top_WithSplit
                    || mode == AttachMode.Bottom_WithSplit)
                    height = (target.Container.View as ILayoutViewWithSize).DesiredHeight * ratio;

                BaseLayoutGroup group;
                BaseGroupControl ctrl;
                if (source.IsDocument)
                {
                    group = new LayoutDocumentGroup(DockMode.Normal, this);
                    ctrl = new LayoutDocumentGroupControl(group, width, height);
                }
                else
                {
                    group = new LayoutGroup(source.Side, DockMode.Normal, this);
                    ctrl = new AnchorSideGroupControl(group, width, height);
                }
                group.Attach(source.ProtoType);
                var _atsource = target.ProtoType.Container.View as IAttcah;
                _atsource.AttachWith(ctrl, mode);
                source.SetActive();
            }
            else throw new ArgumentNullException("the container of source is null!");
        }
        #endregion

        public void Dispose()
        {
            foreach (var ctrl in new List<IDockControl>(_dockControls.Values))
                ctrl.Dispose();
            _dockControls.Clear();
            _dockControls = null;
            foreach (var wnd in new List<BaseFloatWindow>(_floatWindows))
                wnd.Close();
            _floatWindows.Clear();
            _floatWindows = null;
            if (_mainWindow != null)
            {
                _mainWindow.Closing -= OnMainWindowClosing;
                _mainWindow = null;
            }
            Root = null;
            _windows.Clear();
            _windows = null;
            backwards.Clear();
            backwards = null;
            forwards.Clear();
            forwards = null;
        }
    }
}