using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YDock.Enum;
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
                if (_model != value)
                {
                    if (_model != null)
                        _model.View = null;
                    _model = value;
                    if (_model != null)
                        _model.View = this;
                }
            }
        }

        #region SideWidth
        public const double SideWidth = 30;
        public const double SplitterWidth = 6;
        #endregion

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
            DocumentTabs = new DocumentTabControl(((RootPanel)Model).Tab);
            LeftSideContent.PropertyChanged += SideContentChanged;
            RightSideContent.PropertyChanged += SideContentChanged;
            TopSideContent.PropertyChanged += SideContentChanged;
            BottomSideContent.PropertyChanged += SideContentChanged;
            DocumentTabs.SizeChanged += DocumentTabs_SizeChanged;
            SizeChanged += OnSizeChanged;
        }

        private void DocumentTabs_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DocumentTabs.ActualWidth < SideWidth)
            {
                var delta = SideWidth - DocumentTabs.ActualWidth;
                if (_internelGrid.ColumnDefinitions[0].ActualWidth - delta >= SideWidth)
                    _internelGrid.ColumnDefinitions[0].Width = new GridLength(_internelGrid.ColumnDefinitions[0].ActualWidth - delta);
                if (_internelGrid.ColumnDefinitions[4].ActualWidth - delta >= SideWidth)
                    _internelGrid.ColumnDefinitions[4].Width = new GridLength(_internelGrid.ColumnDefinitions[4].ActualWidth - delta);
            }
            if (DocumentTabs.ActualHeight < SideWidth)
            {
                var delta = SideWidth - DocumentTabs.ActualHeight;
                if (_internelGrid.RowDefinitions[0].ActualHeight - delta >= SideWidth)
                    _internelGrid.RowDefinitions[0].Height = new GridLength(_internelGrid.RowDefinitions[0].ActualHeight - delta);
                if (_internelGrid.RowDefinitions[4].ActualHeight - delta >= SideWidth)
                    _internelGrid.RowDefinitions[4].Height = new GridLength(_internelGrid.RowDefinitions[4].ActualHeight - delta);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                double delta = e.NewSize.Width - e.PreviousSize.Width;
                if (delta > 0)
                {
                    if (LeftSideContent.Model != null && LeftSideContent.ActualWidth < LeftSideContent.Model.Width)
                        _internelGrid.ColumnDefinitions[0].Width = new GridLength(LeftSideContent.ActualWidth + delta);
                    else if (RightSideContent.Model != null && RightSideContent.ActualWidth < RightSideContent.Model.Width)
                        _internelGrid.ColumnDefinitions[4].Width = new GridLength(RightSideContent.ActualWidth + delta);
                }
                else
                {
                    if (DocumentTabs.ActualWidth <= SideWidth)
                    {
                        if (RightSideContent.Model != null && RightSideContent.ActualWidth > SideWidth)
                        {
                            if (RightSideContent.ActualWidth + delta < SideWidth)
                            {
                                delta += RightSideContent.ActualWidth - SideWidth;
                                _internelGrid.ColumnDefinitions[4].Width = new GridLength(SideWidth);
                                if (LeftSideContent.Model != null && LeftSideContent.ActualWidth > SideWidth)
                                {
                                    if (LeftSideContent.ActualWidth + delta < SideWidth)
                                    {
                                        delta += LeftSideContent.ActualWidth - SideWidth;
                                        _internelGrid.ColumnDefinitions[0].Width = new GridLength(SideWidth);
                                    }
                                    else _internelGrid.ColumnDefinitions[0].Width = new GridLength(LeftSideContent.ActualWidth + delta);
                                }
                            }
                            else _internelGrid.ColumnDefinitions[4].Width = new GridLength(RightSideContent.ActualWidth + delta);
                        }
                        else if (LeftSideContent.Model != null && LeftSideContent.ActualWidth > SideWidth)
                        {
                            if (LeftSideContent.ActualWidth + delta < SideWidth)
                            {
                                delta += LeftSideContent.ActualWidth - SideWidth;
                                _internelGrid.ColumnDefinitions[0].Width = new GridLength(SideWidth);
                            }
                            else _internelGrid.ColumnDefinitions[0].Width = new GridLength(LeftSideContent.ActualWidth + delta);
                        }

                        double remain = ActualWidth - SideWidth - (LeftSideContent.Model != null ? _internelGrid.ColumnDefinitions[0].Width.Value + SplitterWidth : 0) - (RightSideContent.Model != null ? _internelGrid.ColumnDefinitions[4].Width.Value + SplitterWidth : 0);

                        if (remain > 0 && RightSideContent.Model != null)
                        {
                            if (_internelGrid.ColumnDefinitions[4].Width.Value + remain > RightSideContent.Model.Width)
                            {
                                remain -= RightSideContent.Model.Width - _internelGrid.ColumnDefinitions[4].Width.Value;
                                _internelGrid.ColumnDefinitions[4].Width = new GridLength(_internelGrid.ColumnDefinitions[4].Width.Value);
                                if (remain > 0 && LeftSideContent.Model != null)
                                {
                                    if (_internelGrid.ColumnDefinitions[0].Width.Value + remain > LeftSideContent.Model.Width)
                                        _internelGrid.ColumnDefinitions[0].Width = new GridLength(LeftSideContent.Model.Width);
                                    else _internelGrid.ColumnDefinitions[0].Width = new GridLength(_internelGrid.ColumnDefinitions[0].Width.Value + remain);
                                }
                            }
                            else _internelGrid.ColumnDefinitions[4].Width = new GridLength(_internelGrid.ColumnDefinitions[4].Width.Value + remain);
                        }
                    }
                }
            }

            if (e.HeightChanged)
            {
                double delta = e.NewSize.Height - e.PreviousSize.Height;
                if (delta > 0)
                {
                    if (TopSideContent.Model != null && TopSideContent.ActualHeight < TopSideContent.Model.Height)
                        _internelGrid.RowDefinitions[0].Height = new GridLength(Math.Min(TopSideContent.ActualHeight + delta, TopSideContent.Model.Height));
                    else if (BottomSideContent.Model != null && BottomSideContent.ActualHeight < BottomSideContent.Model.Height)
                        _internelGrid.RowDefinitions[4].Height = new GridLength(Math.Min(BottomSideContent.ActualHeight + delta, BottomSideContent.Model.Height));
                }
                else
                {
                    if (DocumentTabs.ActualHeight <= SideWidth)
                    {
                        if (BottomSideContent.Model != null && BottomSideContent.ActualHeight > SideWidth)
                        {
                            if (BottomSideContent.ActualHeight + delta < SideWidth)
                            {
                                delta += BottomSideContent.ActualHeight - SideWidth;
                                _internelGrid.RowDefinitions[4].Height = new GridLength(SideWidth);
                                if (TopSideContent.Model != null && TopSideContent.ActualHeight > SideWidth)
                                {
                                    if (TopSideContent.ActualHeight + delta < SideWidth)
                                    {
                                        delta += TopSideContent.ActualHeight - SideWidth;
                                        _internelGrid.RowDefinitions[0].Height = new GridLength(SideWidth);
                                    }
                                    else _internelGrid.RowDefinitions[0].Height = new GridLength(TopSideContent.ActualHeight + delta);
                                }
                            }
                            else _internelGrid.RowDefinitions[4].Height = new GridLength(BottomSideContent.ActualHeight + delta);
                        }
                        else if (TopSideContent.Model != null && TopSideContent.ActualHeight > SideWidth)
                        {
                            if (TopSideContent.ActualHeight + delta < SideWidth)
                            {
                                delta += TopSideContent.ActualHeight - SideWidth;
                                _internelGrid.RowDefinitions[0].Height = new GridLength(SideWidth);
                            }
                            else _internelGrid.RowDefinitions[0].Height = new GridLength(TopSideContent.ActualHeight + delta);
                        }

                        double remain = ActualHeight - SideWidth - (TopSideContent.Model != null ? _internelGrid.RowDefinitions[0].Height.Value + SplitterWidth : 0) - (BottomSideContent.Model != null ? _internelGrid.RowDefinitions[4].Height.Value + SplitterWidth : 0);

                        if (remain > 0 && BottomSideContent.Model != null)
                        {
                            if (_internelGrid.RowDefinitions[4].Height.Value + remain > BottomSideContent.Model.Height)
                            {
                                remain -= BottomSideContent.Model.Height - _internelGrid.RowDefinitions[4].Height.Value;
                                _internelGrid.RowDefinitions[4].Height = new GridLength(_internelGrid.RowDefinitions[4].Height.Value);
                                if (remain > 0 && TopSideContent.Model != null)
                                {
                                    if (_internelGrid.RowDefinitions[0].Height.Value + remain > TopSideContent.Model.Height)
                                        _internelGrid.RowDefinitions[0].Height = new GridLength(TopSideContent.Model.Height);
                                    else _internelGrid.RowDefinitions[0].Height = new GridLength(_internelGrid.RowDefinitions[0].Height.Value + remain);
                                }
                            }
                            else _internelGrid.RowDefinitions[4].Height = new GridLength(_internelGrid.RowDefinitions[4].Height.Value + remain);
                        }
                    }
                }
            }
        }

        private void SideContentChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == LeftSideContent)
            {
                if (_leftSplitter == null) return;
                if (LeftSideContent.Model != null)
                {
                    _leftSplitter.Width = SplitterWidth;
                    if (LeftSideContent.Model.Width > 0)
                        _internelGrid.ColumnDefinitions[0].Width = new GridLength(LeftSideContent.Model.Width);
                }
                else
                {
                    _leftSplitter.Width = 0;
                    _internelGrid.ColumnDefinitions[0].Width = new GridLength();
                }
            }
            if (sender == RightSideContent)
            {
                if (_rightSplitter == null) return;
                if (RightSideContent.Model != null)
                {
                    _rightSplitter.Width = SplitterWidth;
                    if (RightSideContent.Model.Width > 0)
                        _internelGrid.ColumnDefinitions[4].Width = new GridLength(RightSideContent.Model.Width);
                }
                else
                {
                    _rightSplitter.Width = 0;
                    _internelGrid.ColumnDefinitions[4].Width = new GridLength();
                }
            }
            if (sender == TopSideContent)
            {
                if (_topSplitter == null) return;
                if (TopSideContent.Model != null)
                {
                    _topSplitter.Height = SplitterWidth;
                    if (TopSideContent.Model.Height > 0)
                        _internelGrid.RowDefinitions[0].Height = new GridLength(TopSideContent.Model.Height);
                }
                else
                {
                    _topSplitter.Height = 0;
                    _internelGrid.RowDefinitions[0].Height = new GridLength();
                }
            }
            if (sender == BottomSideContent)
            {
                if (_bottomSplitter == null) return;
                if (BottomSideContent.Model != null)
                {
                    _bottomSplitter.Height = SplitterWidth;
                    if (BottomSideContent.Model.Height > 0)
                        _internelGrid.RowDefinitions[4].Height = new GridLength(BottomSideContent.Model.Height);
                }
                else
                {
                    _bottomSplitter.Height = 0;
                    _internelGrid.RowDefinitions[4].Height = new GridLength();
                }
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
        private Window _dragWnd;
        private Rectangle _dragRect;
        private Point _pToInterGrid;

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

        private void _Splitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender == _leftSplitter)
                Canvas.SetLeft(_dragRect, YDockHelper.GetMaxOrMinValue(_pToInterGrid.X + e.HorizontalChange, SideWidth, Math.Max(ActualWidth - SideWidth - 2 * SplitterWidth - RightSideContent.ActualWidth, SideWidth)));
            if (sender == _rightSplitter)
                Canvas.SetLeft(_dragRect, YDockHelper.GetMaxOrMinValue(_pToInterGrid.X + e.HorizontalChange, Math.Min(SideWidth + SplitterWidth + LeftSideContent.ActualWidth, ActualWidth - SideWidth - SplitterWidth), ActualWidth - SideWidth - SplitterWidth));
            if (sender == _topSplitter)
                Canvas.SetTop(_dragRect, YDockHelper.GetMaxOrMinValue(_pToInterGrid.Y + e.VerticalChange, SideWidth, Math.Max(ActualHeight - SideWidth - 2 * SplitterWidth - BottomSideContent.ActualHeight, SideWidth)));
            if (sender == _bottomSplitter)
            {
                double height = SideWidth + SplitterWidth + (TopSideContent.Model != null ? SideWidth + SplitterWidth : 0);
                if (ActualHeight <= height) return;
                Canvas.SetTop(_dragRect, YDockHelper.GetMaxOrMinValue(_pToInterGrid.Y + e.VerticalChange, Math.Min(SideWidth + SplitterWidth + TopSideContent.ActualHeight, ActualHeight - SideWidth - SplitterWidth), ActualHeight - SideWidth - SplitterWidth));
            }
        }

        private void _Splitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            double delta;
            if (sender == _leftSplitter || sender == _rightSplitter)
                delta = Canvas.GetLeft(_dragRect) - _pToInterGrid.X;
            else delta = Canvas.GetTop(_dragRect) - _pToInterGrid.Y;
            if (sender == _leftSplitter)
            {
                _internelGrid.ColumnDefinitions[0].Width = new GridLength(_internelGrid.ColumnDefinitions[0].ActualWidth + delta);
                (LeftSideContent.Model as LayoutElement).Width = _internelGrid.ColumnDefinitions[0].ActualWidth + delta;
            }
            if (sender == _rightSplitter)
            {
                _internelGrid.ColumnDefinitions[4].Width = new GridLength(_internelGrid.ColumnDefinitions[4].ActualWidth - delta);
                (RightSideContent.Model as LayoutElement).Width = _internelGrid.ColumnDefinitions[4].ActualWidth - delta;
            }
            if (sender == _topSplitter)
            {
                _internelGrid.RowDefinitions[0].Height = new GridLength(_internelGrid.RowDefinitions[0].ActualHeight + delta);
                (TopSideContent.Model as LayoutElement).Height = _internelGrid.RowDefinitions[0].ActualHeight + delta;
            }
            if (sender == _bottomSplitter)
            {
                _internelGrid.RowDefinitions[4].Height = new GridLength(_internelGrid.RowDefinitions[4].ActualHeight - delta);
                (BottomSideContent.Model as LayoutElement).Height = _internelGrid.RowDefinitions[4].ActualHeight - delta;
            }
            _DisposeDragWnd();
        }

        private void _Splitter_DragStarted(object sender, DragStartedEventArgs e)
        {
            _CreateDragWnd(sender as LayoutGridSplitter);
        }

        private void _CreateDragWnd(LayoutGridSplitter splitter)
        {
            var canvas = new Canvas()
            {
                Height = ActualHeight,
                Width = ActualWidth
            };
            _dragRect = new Rectangle()
            {
                Fill = splitter.BackgroundWhileDragging,
                Opacity = splitter.OpacityWhileDragging,
                Width = splitter.ActualWidth,
                Height = splitter.ActualHeight
            };
            canvas.Children.Add(_dragRect);

            var pToScreen = _internelGrid.PointToScreenDPIWithoutFlowDirection(new Point());
            var transfrom = splitter.TransformToAncestor(_internelGrid);
            _pToInterGrid = transfrom.Transform(new Point(0, 0));

            Canvas.SetLeft(_dragRect, _pToInterGrid.X);
            Canvas.SetTop(_dragRect, _pToInterGrid.Y);

            _dragWnd = new Window()
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Height = ActualHeight,
                Width = ActualWidth,
                Background = Brushes.Transparent,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                IsHitTestVisible = false,
                ShowActivated = false,
                Content = canvas,
                Top = pToScreen.Y,
                Left = pToScreen.X
            };

            _dragWnd.Show();
        }

        private void _DisposeDragWnd()
        {
            _dragWnd.Close();
            _dragRect = null;
            _dragWnd = null;
        }
    }
}