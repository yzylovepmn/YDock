using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;
using YDock.View;

namespace YDock
{
    [ContentProperty("Root")]
    public class YDock : Control
    {
        static YDock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YDock), new FrameworkPropertyMetadata(typeof(YDock)));
            FocusableProperty.OverrideMetadata(typeof(YDock), new FrameworkPropertyMetadata(false));
        }

        public YDock()
        {
            Root = new YDockRoot();
            _documents = new List<ILayoutElement>();
            _leftChildren = new List<ILayoutElement>();
            _topChildren = new List<ILayoutElement>();
            _rightChildren = new List<ILayoutElement>();
            _bottomChildren = new List<ILayoutElement>();
        }

        #region Root
        private YDockRoot _root;
        public YDockRoot Root
        {
            get { return _root; }
            set
            {
                if (_root != value)
                {
                    if (_root != null)
                        _root.DockManager = null;
                    _root = value;
                    if (_root != null)
                        _root.DockManager = this;
                }
            }
        }
        #endregion


        #region DependencyProperty
        public static readonly DependencyProperty LeftSideProperty =
            DependencyProperty.Register("LeftSide", typeof(DockSideGroupControl), typeof(YDock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnLeftSideChanged)));

        public DockSideGroupControl LeftSide
        {
            get { return (DockSideGroupControl)GetValue(LeftSideProperty); }
            set { SetValue(LeftSideProperty, value); }
        }

        private static void OnLeftSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((YDock)d).OnLeftSideChanged(e);
        }

        protected virtual void OnLeftSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty RightSideProperty =
            DependencyProperty.Register("RightSide", typeof(DockSideGroupControl), typeof(YDock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnRightSideChanged)));

        public DockSideGroupControl RightSide
        {
            get { return (DockSideGroupControl)GetValue(RightSideProperty); }
            set { SetValue(RightSideProperty, value); }
        }

        private static void OnRightSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((YDock)d).OnRightSideChanged(e);
        }

        protected virtual void OnRightSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty TopSideProperty =
            DependencyProperty.Register("TopSide", typeof(DockSideGroupControl), typeof(YDock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnTopSideChanged)));

        public DockSideGroupControl TopSide
        {
            get { return (DockSideGroupControl)GetValue(TopSideProperty); }
            set { SetValue(TopSideProperty, value); }
        }

        private static void OnTopSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((YDock)d).OnTopSideChanged(e);
        }

        protected virtual void OnTopSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty BottomSideProperty =
            DependencyProperty.Register("BottomSide", typeof(DockSideGroupControl), typeof(YDock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnBottomSideChanged)));

        public DockSideGroupControl BottomSide
        {
            get { return (DockSideGroupControl)GetValue(BottomSideProperty); }
            set { SetValue(BottomSideProperty, value); }
        }

        private static void OnBottomSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((YDock)d).OnBottomSideChanged(e);
        }

        protected virtual void OnBottomSideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty LayoutRootPanelProperty =
            DependencyProperty.Register("LayoutRootPanel", typeof(LayoutRootPanel), typeof(YDock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnLayoutRootPanelChanged)));

        public LayoutRootPanel LayoutRootPanel
        {
            get { return (LayoutRootPanel)GetValue(LayoutRootPanelProperty); }
            set { SetValue(LayoutRootPanelProperty, value); }
        }

        private static void OnLayoutRootPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((YDock)d).OnLayoutRootPanelChanged(e);
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
        public ILayoutElement AutoHideElement
        {
            get { return LayoutRootPanel.AHWindow.Model; }
            set
            {
                if (LayoutRootPanel.AHWindow.Model != value)
                    LayoutRootPanel.AHWindow.Model = value as LayoutElement;
                else LayoutRootPanel.AHWindow.Model = null;
            }
        }

        private IList<ILayoutElement> _documents;
        public IEnumerable<ILayoutElement> Documents
        {
            get { return _documents; }
        }

        private IList<ILayoutElement> _leftChildren;
        public IEnumerable<ILayoutElement> LeftChildren
        {
            get { return _leftChildren; }
        }

        private IList<ILayoutElement> _rightChildren;
        public IEnumerable<ILayoutElement> RightChildren
        {
            get { return _rightChildren; }
        }

        private IList<ILayoutElement> _topChildren;
        public IEnumerable<ILayoutElement> TopChildren
        {
            get { return _topChildren; }
        }

        private IList<ILayoutElement> _bottomChildren;
        public IEnumerable<ILayoutElement> BottomChildren
        {
            get { return _bottomChildren; }
        }

        public IEnumerable<ILayoutElement> Children
        {
            get
            {
                foreach (var item in _documents)
                    yield return item;
                foreach (var item in _leftChildren)
                    yield return item;
                foreach (var item in _topChildren)
                    yield return item;
                foreach (var item in _rightChildren)
                    yield return item;
                foreach (var item in _bottomChildren)
                    yield return item;
            }
        }

        public void AddDocument(ILayoutElement document)
        {
            LayoutRootPanel.RootGroupPanel.ContainDocument = true;
            var group = new LayoutDocumentGroup(this);
            group.Children.Add(document);
            if (LayoutRootPanel.RootGroupPanel.IsEmpty)
                LayoutRootPanel.RootGroupPanel.AddDocumentChild(new LayoutDocumentGroupControl(group) { DesiredHeight = document.DesiredHeight, DesiredWidth = document.DesiredWidth });
            else LayoutRootPanel.RootGroupPanel.AddDocumentChild(new LayoutDocumentGroupControl(group));
        }

        public void AddAnchorChild(ILayoutElement child)
        {
            if (child.Side == DockSide.Left ||
                child.Side == DockSide.Right)
            {
                if (LayoutRootPanel.RootGroupPanel.Children.Count == 1 || LayoutRootPanel.RootGroupPanel.Direction == Direction.LeftToRight)
                {
                    LayoutRootPanel.RootGroupPanel.Direction = Direction.LeftToRight;
                    LayoutRootPanel.RootGroupPanel.IsDocumentPanel = false;
                    var group = new LayoutGroup(child.Side, this);
                    group.Children.Add(child);
                    LayoutRootPanel.RootGroupPanel.AddAnchorChild(new AnchorSideGroupControl(group) { DesiredHeight = child.DesiredHeight, DesiredWidth = child.DesiredWidth }, child.Side);
                }
                else
                {
                    LayoutGroupPanel rootPanel = new LayoutGroupPanel() { ContainDocument = true, Direction = Direction.LeftToRight };
                    var oldrootPanel = LayoutRootPanel.RootGroupPanel;
                    LayoutRootPanel.RootGroupPanel = null;
                    rootPanel.AddChild(oldrootPanel);

                    var group = new LayoutGroup(child.Side, this);
                    group.Children.Add(child);
                    rootPanel.AddAnchorChild(new AnchorSideGroupControl(group) { DesiredHeight = child.DesiredHeight, DesiredWidth = child.DesiredWidth }, child.Side);
                    LayoutRootPanel.RootGroupPanel = rootPanel;
                }
            }
            else
            {
                if (LayoutRootPanel.RootGroupPanel.Children.Count == 1 || LayoutRootPanel.RootGroupPanel.Direction == Direction.UpToDown)
                {
                    LayoutRootPanel.RootGroupPanel.Direction = Direction.UpToDown;
                    LayoutRootPanel.RootGroupPanel.IsDocumentPanel = false;
                    var group = new LayoutGroup(child.Side, this);
                    group.Children.Add(child);
                    LayoutRootPanel.RootGroupPanel.AddAnchorChild(new AnchorSideGroupControl(group) { DesiredHeight = child.DesiredHeight, DesiredWidth = child.DesiredWidth }, child.Side);
                }
                else
                {
                    LayoutGroupPanel rootPanel = new LayoutGroupPanel() { ContainDocument = true, Direction = Direction.UpToDown };
                    var oldrootPanel = LayoutRootPanel.RootGroupPanel;
                    LayoutRootPanel.RootGroupPanel = null;
                    rootPanel.AddChild(oldrootPanel);

                    var group = new LayoutGroup(child.Side, this);
                    group.Children.Add(child);
                    rootPanel.AddAnchorChild(new AnchorSideGroupControl(group) { DesiredHeight = child.DesiredHeight, DesiredWidth = child.DesiredWidth }, child.Side);
                    LayoutRootPanel.RootGroupPanel = rootPanel;
                }
            }
        }

        public void AddSidePanel(DockSide side)
        {
            switch (side)
            {
                case DockSide.Left:
                    LayoutGroupPanel rootPanel = new LayoutGroupPanel() { ContainDocument = false, IsAnchorPanel = true, Direction = Direction.UpToDown, DesiredWidth = 200 };

                    var child = new LayoutElement() { Side = DockSide.Left, Title = "Document_Left", DesiredHeight = 100 };
                    var group = new LayoutGroup(child.Side, this);
                    group.Children.Add(child);
                    rootPanel.Children.Add(new AnchorSideGroupControl(group) { DesiredHeight = child.DesiredHeight, DesiredWidth = child.DesiredWidth });
                    rootPanel.Children.Add(rootPanel._CreateSplitter(Cursors.SizeNS));
                    child = new LayoutElement() { Side = DockSide.Left, Title = "Document_Left", DesiredHeight = 180 };
                    group = new LayoutGroup(child.Side, this);
                    group.Children.Add(child);
                    rootPanel.Children.Add(new AnchorSideGroupControl(group) { DesiredHeight = child.DesiredHeight, DesiredWidth = child.DesiredWidth });
                    rootPanel.Children.Add(rootPanel._CreateSplitter(Cursors.SizeNS));
                    child = new LayoutElement() { Side = DockSide.Left, Title = "Document_Left", DesiredHeight = 120 };
                    group = new LayoutGroup(child.Side, this);
                    group.Children.Add(child);
                    rootPanel.Children.Add(new AnchorSideGroupControl(group) { DesiredHeight = child.DesiredHeight, DesiredWidth = child.DesiredWidth });

                    LayoutGroupPanel newrootPanel = new LayoutGroupPanel() { ContainDocument = true, Direction = Direction.LeftToRight };
                    var oldrootPanel = LayoutRootPanel.RootGroupPanel;
                    LayoutRootPanel.RootGroupPanel = null;
                    newrootPanel.AddChild(oldrootPanel);
                    newrootPanel.AddAnchorChild(rootPanel, side);
                    LayoutRootPanel.RootGroupPanel = newrootPanel;
                    break;
                case DockSide.Right:
                    break;
                case DockSide.Top:
                    break;
                case DockSide.Bottom:
                    break;
            }
        }
    }
}