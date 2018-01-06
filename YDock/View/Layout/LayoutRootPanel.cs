using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class LayoutRootPanel : Panel, IView
    {
        static LayoutRootPanel()
        {
            FocusableProperty.OverrideMetadata(typeof(LayoutRootPanel), new FrameworkPropertyMetadata(false));
        }

        public LayoutRootPanel(IModel model)
        {
            Model = model;
        }

        #region SideWidth & SplitterWidth
        public const double SideWidth = 30;
        public const double SplitterWidth = 6;
        #endregion

        private AnchorDocumentControl _topSideContent;
        public AnchorDocumentControl TopSideContent
        {
            get { return _topSideContent; }
            set
            {
                if (_topSideContent != value)
                {
                    if (_topSideContent != null)
                        Children.Remove(_topSideContent);
                    _topSideContent = value;
                    if (_topSideContent != null)
                        Children.Add(_topSideContent);
                }
            }
        }

        private AnchorDocumentControl _bottomSideContent;
        public AnchorDocumentControl BottomSideContent
        {
            get { return _bottomSideContent; }
            set
            {
                if (_bottomSideContent != value)
                {
                    if (_bottomSideContent != null)
                        Children.Remove(_bottomSideContent);
                    _bottomSideContent = value;
                    if (_bottomSideContent != null)
                        Children.Add(_bottomSideContent);
                }
            }
        }

        private AnchorDocumentControl _leftSideContent;
        public AnchorDocumentControl LeftSideContent
        {
            get { return _leftSideContent; }
            set
            {
                if (_leftSideContent != value)
                {
                    if (_leftSideContent != null)
                        Children.Remove(_leftSideContent);
                    _leftSideContent = value;
                    if (_leftSideContent != null)
                        Children.Add(_leftSideContent);
                }
            }
        }

        private AnchorDocumentControl _rightSideContent;
        public AnchorDocumentControl RightSideContent
        {
            get { return _rightSideContent; }
            set
            {
                if (_rightSideContent != value)
                {
                    if (_rightSideContent != null)
                        Children.Remove(_rightSideContent);
                    _rightSideContent = value;
                    if (_rightSideContent != null)
                        Children.Add(_rightSideContent);
                }
            }
        }

        private DocumentTabControl _documentTabs;
        public DocumentTabControl DocumentTabs
        {
            get { return _documentTabs; }
            set
            {
                if (_documentTabs != value)
                {
                    if (_documentTabs != null)
                        Children.Remove(_documentTabs);
                    _documentTabs = value;
                    if (_documentTabs != null)
                        Children.Add(_documentTabs);
                }
            }
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

        private LayoutGridSplitter _leftSplitter;
        private LayoutGridSplitter _rightSplitter;
        private LayoutGridSplitter _topSplitter;
        private LayoutGridSplitter _bottomSplitter;
        private Window _dragWnd;
        private Rectangle _dragRect;
        private Point _pToInterGrid;

        private void _SetupSplitter()
        {
            _leftSplitter = new LayoutGridSplitter() { VerticalAlignment = VerticalAlignment.Stretch, Cursor = Cursors.SizeWE };
            _rightSplitter = new LayoutGridSplitter() { VerticalAlignment = VerticalAlignment.Stretch, Cursor = Cursors.SizeWE };
            _topSplitter = new LayoutGridSplitter() { HorizontalAlignment = HorizontalAlignment.Stretch, Cursor = Cursors.SizeNS };
            _bottomSplitter = new LayoutGridSplitter() { HorizontalAlignment = HorizontalAlignment.Stretch, Cursor = Cursors.SizeNS };


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
                (LeftSideContent.Model as LayoutElement).ActualWidth = _leftSideContent.ActualWidth + delta;
            if (sender == _rightSplitter)
                (RightSideContent.Model as LayoutElement).ActualWidth = _rightSideContent.ActualWidth - delta;
            if (sender == _topSplitter)
                (TopSideContent.Model as LayoutElement).ActualHeight = _topSideContent.ActualHeight + delta;
            if (sender == _bottomSplitter)
                (BottomSideContent.Model as LayoutElement).ActualHeight = _bottomSideContent.ActualHeight - delta;
            if (delta != 0)
                InvalidateMeasure();
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

            var pToScreen = this.PointToScreenDPIWithoutFlowDirection(new Point());
            var transfrom = splitter.TransformToAncestor(this);
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

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _SetupSplitter();

            TopSideContent = new AnchorDocumentControl();
            Children.Add(_topSplitter);
            LeftSideContent = new AnchorDocumentControl();
            Children.Add(_leftSplitter);
            DocumentTabs = new DocumentTabControl(((RootPanel)Model).Tab);
            Children.Add(_rightSplitter);
            RightSideContent = new AnchorDocumentControl();
            Children.Add(_bottomSplitter);
            BottomSideContent = new AnchorDocumentControl();

            LeftSideContent.PropertyChanged += SideContentChanged;
            RightSideContent.PropertyChanged += SideContentChanged;
            TopSideContent.PropertyChanged += SideContentChanged;
            BottomSideContent.PropertyChanged += SideContentChanged;
        }

        private void SideContentChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double up = 0;
            double down = 0;
            double left = 0;
            double right = 0;

            if (_topSideContent.Model != null)
            {
                if (YDockHelper.IsSizeEmpty(_topSideContent.Model.ActualWidth, _topSideContent.Model.ActualHeight))
                {
                    (_topSideContent.Model as LayoutElement).ActualWidth = availableSize.Width;
                    (_topSideContent.Model as LayoutElement).ActualHeight = Math.Max(_topSideContent.DesiredSize.Height, SideWidth);
                }
                up += (_topSideContent.Model as LayoutElement).ActualHeight;
                up += SplitterWidth;
            }

            if (_bottomSideContent.Model != null)
            {
                if (YDockHelper.IsSizeEmpty(_bottomSideContent.Model.ActualWidth, _bottomSideContent.Model.ActualHeight))
                {
                    (_bottomSideContent.Model as LayoutElement).ActualWidth = availableSize.Width;
                    (_bottomSideContent.Model as LayoutElement).ActualHeight = Math.Max(_bottomSideContent.DesiredSize.Height, SideWidth);
                }
                down += (_bottomSideContent.Model as LayoutElement).ActualHeight;
                down += SplitterWidth;
            }

            if (up + down + SideWidth > availableSize.Height)
            {
                double delta = up + down + SideWidth - availableSize.Height;
                if (up > 0)
                {
                    if (up - delta >= SideWidth + SplitterWidth)
                    {
                        up = up - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= up - (SideWidth + SplitterWidth);
                        up = SideWidth + SplitterWidth;
                    }
                }
                if (delta > 0 && down > 0)
                {
                    if (down - delta >= SideWidth + SplitterWidth)
                    {
                        down = down - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= down - (SideWidth + SplitterWidth);
                        down = SideWidth + SplitterWidth;
                    }
                }
            }

            if (up > 0)
            {
                _topSideContent.Measure(new Size(availableSize.Width, up - SplitterWidth));
                _topSplitter.Measure(new Size(availableSize.Width, SplitterWidth));
            }
            else
            {
                _topSideContent.Measure(new Size());
                _topSplitter.Measure(new Size());
            }
            if (down > 0)
            {
                _bottomSideContent.Measure(new Size(availableSize.Width, down - SplitterWidth));
                _bottomSplitter.Measure(new Size(availableSize.Width, SplitterWidth));
            }
            else
            {
                _bottomSideContent.Measure(new Size());
                _bottomSplitter.Measure(new Size());
            }


            if (_leftSideContent.Model != null)
            {
                if (YDockHelper.IsSizeEmpty(_leftSideContent.Model.ActualWidth, _leftSideContent.Model.ActualHeight))
                {
                    (_leftSideContent.Model as LayoutElement).ActualWidth = Math.Max(_leftSideContent.DesiredSize.Width, SideWidth);
                    (_leftSideContent.Model as LayoutElement).ActualHeight = Math.Max(availableSize.Height - up - down, 0);
                }
                left += (_leftSideContent.Model as LayoutElement).ActualWidth;
                left += SplitterWidth;
            }

            if (_rightSideContent.Model != null)
            {
                if (YDockHelper.IsSizeEmpty(_rightSideContent.Model.ActualWidth, _rightSideContent.Model.ActualHeight))
                {
                    (_rightSideContent.Model as LayoutElement).ActualWidth = Math.Max(_rightSideContent.DesiredSize.Width, SideWidth);
                    (_rightSideContent.Model as LayoutElement).ActualHeight = Math.Max(availableSize.Height - up - down, 0);
                }
                right += (_rightSideContent.Model as LayoutElement).ActualWidth;
                right += SplitterWidth;
            }

            if (left + right + SideWidth > availableSize.Width)
            {
                double delta = left + right + SideWidth - availableSize.Width;
                if (left > 0)
                {
                    if (left - delta >= SideWidth + SplitterWidth)
                    {
                        left = left - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= left - (SideWidth + SplitterWidth);
                        left = SideWidth + SplitterWidth;
                    }
                }
                if (delta > 0 && right > 0)
                {
                    if (right - delta >= SideWidth + SplitterWidth)
                    {
                        right = right - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= right - (SideWidth + SplitterWidth);
                        right = SideWidth + SplitterWidth;
                    }
                }
            }

            if (left > 0)
            {
                _leftSideContent.Measure(new Size(left - SplitterWidth, Math.Max(availableSize.Height - up - down, 0)));
                _leftSplitter.Measure(new Size(SplitterWidth, Math.Max(availableSize.Height - up - down, 0)));
            }
            else
            {
                _leftSideContent.Measure(new Size());
                _leftSplitter.Measure(new Size());
            }
            if (right > 0)
            {
                _rightSideContent.Measure(new Size(right - SplitterWidth, Math.Max(availableSize.Height - up - down, 0)));
                _rightSplitter.Measure(new Size(SplitterWidth, Math.Max(availableSize.Height - up - down, 0)));
            }
            else
            {
                _rightSideContent.Measure(new Size());
                _rightSplitter.Measure(new Size());
            }

            _documentTabs.Measure(new Size(Math.Max(availableSize.Width - left - right, 0), Math.Max(availableSize.Height - up - down, 0)));

            return availableSize;
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            double up = 0;
            double down = 0;
            double left = 0;
            double right = 0;

            if (_topSideContent.Model != null)
            {
                up += (_topSideContent.Model as LayoutElement).ActualHeight;
                up += SplitterWidth;
            }

            if (_bottomSideContent.Model != null)
            {
                down += (_bottomSideContent.Model as LayoutElement).ActualHeight;
                down += SplitterWidth;
            }

            if (up + down + SideWidth > finalSize.Height)
            {
                double delta = up + down + SideWidth - finalSize.Height;
                if (up > 0)
                {
                    if (up - delta >= SideWidth + SplitterWidth)
                    {
                        up = up - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= up - (SideWidth + SplitterWidth);
                        up = SideWidth + SplitterWidth;
                    }
                }
                if (delta > 0 && down > 0)
                {
                    if (down - delta >= SideWidth + SplitterWidth)
                    {
                        down = down - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= down - (SideWidth + SplitterWidth);
                        down = SideWidth + SplitterWidth;
                    }
                }
            }

            if (up > 0)
            {
                _topSideContent.Arrange(new Rect(0, 0, finalSize.Width, up - SplitterWidth));
                _topSplitter.Arrange(new Rect(0, up - SplitterWidth, finalSize.Width, SplitterWidth));
            }
            else
            {
                _topSideContent.Arrange(new Rect());
                _topSplitter.Arrange(new Rect());
            }
            if (down > 0)
            {
                if (up + down > finalSize.Height)
                {
                    _bottomSideContent.Arrange(new Rect(0, up + SplitterWidth, finalSize.Width, down - SplitterWidth));
                    _bottomSplitter.Arrange(new Rect(0, up, finalSize.Width, SplitterWidth));
                }
                else
                {
                    _bottomSideContent.Arrange(new Rect(0, finalSize.Height - (down - SplitterWidth), finalSize.Width, down - SplitterWidth));
                    _bottomSplitter.Arrange(new Rect(0, finalSize.Height - down, finalSize.Width, SplitterWidth));
                }
            }
            else
            {
                _bottomSideContent.Arrange(new Rect());
                _bottomSplitter.Arrange(new Rect());
            }



            if (_leftSideContent.Model != null)
            {
                left += (_leftSideContent.Model as LayoutElement).ActualWidth;
                left += SplitterWidth;
            }

            if (_rightSideContent.Model != null)
            {
                right += (_rightSideContent.Model as LayoutElement).ActualWidth;
                right += SplitterWidth;
            }

            if (left + right + SideWidth > finalSize.Width)
            {
                double delta = left + right + SideWidth - finalSize.Width;
                if (left > 0)
                {
                    if (left - delta >= SideWidth + SplitterWidth)
                    {
                        left = left - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= left - (SideWidth + SplitterWidth);
                        left = SideWidth + SplitterWidth;
                    }
                }
                if (delta > 0 && right > 0)
                {
                    if (right - delta >= SideWidth + SplitterWidth)
                    {
                        right = right - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= right - (SideWidth + SplitterWidth);
                        right = SideWidth + SplitterWidth;
                    }
                }
            }

            double height = Math.Max(finalSize.Height - up - down, 0);
            if (left > 0)
            {
                _leftSideContent.Arrange(new Rect(0, up, left - SplitterWidth, height));
                _leftSplitter.Arrange(new Rect(left - SplitterWidth, up, SplitterWidth, height));
            }
            else
            {
                _leftSideContent.Arrange(new Rect());
                _leftSplitter.Arrange(new Rect());
            }
            if (right > 0)
            {
                if (left + right > finalSize.Width)
                {
                    _rightSideContent.Arrange(new Rect(left + SplitterWidth, up, right - SplitterWidth, height));
                    _rightSplitter.Arrange(new Rect(left, up, SplitterWidth, height));
                }
                else
                {
                    _rightSideContent.Arrange(new Rect(finalSize.Width - (right - SplitterWidth), up, right - SplitterWidth, height));
                    _rightSplitter.Arrange(new Rect(finalSize.Width - right, up, SplitterWidth, height));
                }
            }
            else
            {
                _rightSideContent.Arrange(new Rect());
                _rightSplitter.Arrange(new Rect());
            }

            _documentTabs.Arrange(new Rect(left, up, finalSize.Width - left - right > 0 ? finalSize.Width - left - right : 0, finalSize.Height - up - down > 0 ? finalSize.Height - up - down : 0));

            return finalSize;
        }
        #endregion
    }
}