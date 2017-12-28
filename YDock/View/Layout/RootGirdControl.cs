using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    [TemplatePart(Name = "PART_Gird", Type = typeof(Grid))]
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
            LeftSideContent.PropertyChanged += SideContentChanged;
            RightSideContent.PropertyChanged += SideContentChanged;
            TopSideContent.PropertyChanged += SideContentChanged;
            BottomSideContent.PropertyChanged += SideContentChanged;
        }

        private void SideContentChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == LeftSideContent)
            {
                if (_leftSplitter == null) return;
                if (LeftSideContent.Model != null)
                    _leftSplitter.Width = 4;
                else _leftSplitter.Width = 0;
            }
            if (sender == RightSideContent)
            {
                if (_rightSplitter == null) return;
                if (RightSideContent.Model != null)
                    _rightSplitter.Width = 4;
                else _rightSplitter.Width = 0;
            }
            if (sender == TopSideContent)
            {
                if (_topSplitter == null) return;
                if (TopSideContent.Model != null)
                    _topSplitter.Height = 4;
                else _topSplitter.Height = 0;
            }
            if (sender == BottomSideContent)
            {
                if (_bottomSplitter == null) return;
                if (BottomSideContent.Model != null)
                    _bottomSplitter.Height = 4;
                else _bottomSplitter.Height = 0;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _SetupSplitter();
        }

        private Grid _internelGrid;
        private LayoutGridSplitter _leftSplitter;
        private LayoutGridSplitter _rightSplitter;
        private LayoutGridSplitter _topSplitter;
        private LayoutGridSplitter _bottomSplitter;


        private void _SetupSplitter()
        {
            _leftSplitter = new LayoutGridSplitter() { VerticalAlignment = VerticalAlignment.Stretch, Width = LeftSideContent.Model == null ? 0 : 4, Cursor = Cursors.SizeWE };
            _rightSplitter = new LayoutGridSplitter() { VerticalAlignment = VerticalAlignment.Stretch, Width = RightSideContent.Model == null ? 0 : 4, Cursor = Cursors.SizeWE };
            _topSplitter = new LayoutGridSplitter() { HorizontalAlignment = HorizontalAlignment.Stretch, Height = TopSideContent.Model == null ? 0 : 4, Cursor = Cursors.SizeNS };
            _bottomSplitter = new LayoutGridSplitter() { HorizontalAlignment = HorizontalAlignment.Stretch, Height = BottomSideContent.Model == null ? 0 : 4, Cursor = Cursors.SizeNS };

            _internelGrid = YDockHelper.GetTemplateChild<Grid>(Template, this, "PART_Gird");
            _internelGrid.Children.Add(_leftSplitter);
            _internelGrid.Children.Add(_rightSplitter);
            _internelGrid.Children.Add(_topSplitter);
            _internelGrid.Children.Add(_bottomSplitter);

            Grid.SetColumn(_leftSplitter, 1);
            Grid.SetRow(_leftSplitter, 2);

            Grid.SetColumn(_rightSplitter, 3);
            Grid.SetRow(_rightSplitter, 2);

            Grid.SetRow(_topSplitter, 1);
            Grid.SetColumnSpan(_topSplitter, 5);

            Grid.SetRow(_bottomSplitter, 3);
            Grid.SetColumnSpan(_bottomSplitter, 5);


            _leftSplitter.DragStarted += _Splitter_DragStarted;
            _leftSplitter.DragCompleted += _Splitter_DragCompleted;
            _leftSplitter.DragDelta += _Splitter_DragDelta;
            _rightSplitter.DragStarted += _Splitter_DragStarted;
            _rightSplitter.DragCompleted += _Splitter_DragCompleted;
            _rightSplitter.DragDelta += _Splitter_DragDelta;
            _topSplitter.DragStarted += _Splitter_DragStarted;
            _topSplitter.DragCompleted += _Splitter_DragCompleted;
            _topSplitter.DragDelta += _Splitter_DragDelta;
            _bottomSplitter.DragStarted += _Splitter_DragStarted;
            _bottomSplitter.DragCompleted += _Splitter_DragCompleted;
            _bottomSplitter.DragDelta += _Splitter_DragDelta;
        }

        private void _Splitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            
        }

        private void _Splitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            
        }

        private void _Splitter_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {

        }
    }
}