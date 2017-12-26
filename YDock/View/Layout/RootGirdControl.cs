using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class RootGirdControl : Control, IView
    {
        static RootGirdControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RootGirdControl), new FrameworkPropertyMetadata(typeof(RootGirdControl)));
            FocusableProperty.OverrideMetadata(typeof(RootGirdControl), new FrameworkPropertyMetadata(false));
        }

        public RootGirdControl(IModel model)
        {
            Model = model;
        }

        private IModel _model;
        public IModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model != null) _model.View = null;
                if (_model != value)
                {
                    _model = value;
                    _model.View = this;
                }
            }
        }

        public static readonly DependencyProperty TopSideContentProperty =
            DependencyProperty.Register("TopSideContent", typeof(AnchorDocumentControl), typeof(RootGirdControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTopSideContentChanged)));

        public AnchorDocumentControl TopSideContent
        {
            get { return (AnchorDocumentControl)GetValue(TopSideContentProperty); }
            set { SetValue(TopSideContentProperty, value); }
        }

        private static void OnTopSideContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RootGirdControl)d).OnTopSideContentChanged(e);
        }

        protected virtual void OnTopSideContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty BottomSideContentProperty =
            DependencyProperty.Register("BottomSideContent", typeof(AnchorDocumentControl), typeof(RootGirdControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBottomSideContentChanged)));

        public AnchorDocumentControl BottomSideContent
        {
            get { return (AnchorDocumentControl)GetValue(BottomSideContentProperty); }
            set { SetValue(BottomSideContentProperty, value); }
        }

        private static void OnBottomSideContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RootGirdControl)d).OnBottomSideContentChanged(e);
        }

        protected virtual void OnBottomSideContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty LeftSideContentProperty =
            DependencyProperty.Register("LeftSideContent", typeof(AnchorDocumentControl), typeof(RootGirdControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLeftSideContentChanged)));

        public AnchorDocumentControl LeftSideContent
        {
            get { return (AnchorDocumentControl)GetValue(LeftSideContentProperty); }
            set { SetValue(LeftSideContentProperty, value); }
        }

        private static void OnLeftSideContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RootGirdControl)d).OnLeftSideContentChanged(e);
        }

        protected virtual void OnLeftSideContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }

        public static readonly DependencyProperty RightSideContentProperty =
            DependencyProperty.Register("RightSideContent", typeof(AnchorDocumentControl), typeof(RootGirdControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRightSideContentChanged)));

        public AnchorDocumentControl RightSideContent
        {
            get { return (AnchorDocumentControl)GetValue(RightSideContentProperty); }
            set { SetValue(RightSideContentProperty, value); }
        }

        private static void OnRightSideContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RootGirdControl)d).OnRightSideContentChanged(e);
        }

        protected virtual void OnRightSideContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }


        public static readonly DependencyProperty DocumentTabsProperty =
            DependencyProperty.Register("DocumentTabs", typeof(DocumentTabControl), typeof(RootGirdControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDocumentTabsChanged)));

        public DocumentTabControl DocumentTabs
        {
            get { return (DocumentTabControl)GetValue(DocumentTabsProperty); }
            set { SetValue(DocumentTabsProperty, value); }
        }

        private static void OnDocumentTabsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RootGirdControl)d).OnDocumentTabsChanged(e);
        }

        protected virtual void OnDocumentTabsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                AddLogicalChild(e.NewValue);
        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            LeftSideContent = new AnchorDocumentControl();
            RightSideContent = new AnchorDocumentControl();
            TopSideContent = new AnchorDocumentControl();
            BottomSideContent = new AnchorDocumentControl();
            DocumentTabs = new DocumentTabControl(((RootGird)Model).Tab);
        }
    }
}