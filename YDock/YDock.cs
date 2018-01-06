using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
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
            DependencyProperty.Register("LeftSide", typeof(YDockSideControl), typeof(YDock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnLeftSideChanged)));

        public YDockSideControl LeftSide
        {
            get { return (YDockSideControl)GetValue(LeftSideProperty); }
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
            DependencyProperty.Register("RightSide", typeof(YDockSideControl), typeof(YDock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnRightSideChanged)));

        public YDockSideControl RightSide
        {
            get { return (YDockSideControl)GetValue(RightSideProperty); }
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
            DependencyProperty.Register("TopSide", typeof(YDockSideControl), typeof(YDock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnTopSideChanged)));

        public YDockSideControl TopSide
        {
            get { return (YDockSideControl)GetValue(TopSideProperty); }
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
            DependencyProperty.Register("BottomSide", typeof(YDockSideControl), typeof(YDock),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnBottomSideChanged)));

        public YDockSideControl BottomSide
        {
            get { return (YDockSideControl)GetValue(BottomSideProperty); }
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

        //public static readonly DependencyProperty CenterGirdProperty =
        //    DependencyProperty.Register("CenterGird", typeof(RootGirdControl), typeof(YDock),
        //        new FrameworkPropertyMetadata(null,
        //            new PropertyChangedCallback(OnCenterGirdChanged)));

        //public RootGirdControl CenterGird
        //{
        //    get { return (RootGirdControl)GetValue(CenterGirdProperty); }
        //    set { SetValue(CenterGirdProperty, value); }
        //}

        //private static void OnCenterGirdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((YDock)d).OnCenterGirdChanged(e);
        //}

        //protected virtual void OnCenterGirdChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.OldValue != null)
        //        RemoveLogicalChild(e.OldValue);
        //    if (e.NewValue != null)
        //        AddLogicalChild(e.NewValue);
        //}

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
                //CenterGird = new RootGirdControl(_root.RootGrid);
                LayoutRootPanel = new LayoutRootPanel(_root.RootPanel);
                LeftSide = new YDockSideControl(_root.LeftSide);
                RightSide = new YDockSideControl(_root.RightSide);
                BottomSide = new YDockSideControl(_root.BottomSide);
                TopSide = new YDockSideControl(_root.TopSide);
            }
        }
    }
}