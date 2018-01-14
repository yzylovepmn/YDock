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
using YDock.Enum;
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

        private AnchorSidePanel _topSideContent;
        public AnchorSidePanel TopSideContent
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

        private AnchorSidePanel _bottomSideContent;
        public AnchorSidePanel BottomSideContent
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

        private AnchorSidePanel _leftSideContent;
        public AnchorSidePanel LeftSideContent
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

        private AnchorSidePanel _rightSideContent;
        public AnchorSidePanel RightSideContent
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
                Canvas.SetLeft(_dragRect, YDockHelper.GetMaxOrMinValue(_pToInterGrid.X + e.HorizontalChange, Constants.SideLength, Math.Max(ActualWidth - Constants.SideLength - 2 * Constants.SplitterSpan - RightSideContent.ActualWidth, Constants.SideLength)));
            if (sender == _rightSplitter)
                Canvas.SetLeft(_dragRect, YDockHelper.GetMaxOrMinValue(_pToInterGrid.X + e.HorizontalChange, Math.Min(Constants.SideLength + Constants.SplitterSpan + LeftSideContent.ActualWidth, ActualWidth - Constants.SideLength - Constants.SplitterSpan), ActualWidth - Constants.SideLength - Constants.SplitterSpan));
            if (sender == _topSplitter)
                Canvas.SetTop(_dragRect, YDockHelper.GetMaxOrMinValue(_pToInterGrid.Y + e.VerticalChange, Constants.SideLength, Math.Max(ActualHeight - Constants.SideLength - 2 * Constants.SplitterSpan - BottomSideContent.ActualHeight, Constants.SideLength)));
            if (sender == _bottomSplitter)
            {
                double height = Constants.SideLength + Constants.SplitterSpan + (TopSideContent.HasContent ? Constants.SideLength + Constants.SplitterSpan : 0);
                if (ActualHeight <= height) return;
                Canvas.SetTop(_dragRect, YDockHelper.GetMaxOrMinValue(_pToInterGrid.Y + e.VerticalChange, Math.Min(Constants.SideLength + Constants.SplitterSpan + TopSideContent.ActualHeight, ActualHeight - Constants.SideLength - Constants.SplitterSpan), ActualHeight - Constants.SideLength - Constants.SplitterSpan));
            }
        }

        private void _Splitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            double delta;
            if (sender == _leftSplitter || sender == _rightSplitter)
                delta = Canvas.GetLeft(_dragRect) - _pToInterGrid.X;
            else delta = Canvas.GetTop(_dragRect) - _pToInterGrid.Y;
            if (sender == _leftSplitter)
                LeftSideContent.ContentSideLenght = _leftSideContent.ActualWidth + delta;
            if (sender == _rightSplitter)
                RightSideContent.ContentSideLenght = _rightSideContent.ActualWidth - delta;
            if (sender == _topSplitter)
                TopSideContent.ContentSideLenght = _topSideContent.ActualHeight + delta;
            if (sender == _bottomSplitter)
                BottomSideContent.ContentSideLenght = _bottomSideContent.ActualHeight - delta;
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

            TopSideContent = new AnchorSidePanel(Direction.LeftToRight);
            Children.Add(_topSplitter);
            LeftSideContent = new AnchorSidePanel(Direction.UpToDown);
            Children.Add(_leftSplitter);
            DocumentTabs = new DocumentTabControl(((RootPanel)Model).Tab);
            Children.Add(_rightSplitter);
            RightSideContent = new AnchorSidePanel(Direction.UpToDown);
            Children.Add(_bottomSplitter);
            BottomSideContent = new AnchorSidePanel(Direction.LeftToRight);

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
            {
                if (child is AnchorSidePanel)
                {
                    var panel = child as AnchorSidePanel;
                    if (panel.Direction == Direction.LeftToRight)
                        panel.Measure(new Size(availableSize.Width, panel.ContentSideLenght));
                    else panel.Measure(new Size(panel.ContentSideLenght, availableSize.Height));
                }
                else child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            double up = 0;
            double down = 0;
            double left = 0;
            double right = 0;

            if (_topSideContent.HasContent)
            {
                up += _topSideContent.ContentSideLenght;
                up += Constants.SplitterSpan;
            }

            if (_bottomSideContent.HasContent)
            {
                down += _bottomSideContent.ContentSideLenght;
                down += Constants.SplitterSpan;
            }

            if (up + down + Constants.SideLength > availableSize.Height)
            {
                double delta = up + down + Constants.SideLength - availableSize.Height;
                if (up > 0)
                {
                    if (up - delta >= Constants.SideLength + Constants.SplitterSpan)
                    {
                        up = up - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= up - (Constants.SideLength + Constants.SplitterSpan);
                        up = Constants.SideLength + Constants.SplitterSpan;
                    }
                }
                if (delta > 0 && down > 0)
                {
                    if (down - delta >= Constants.SideLength + Constants.SplitterSpan)
                    {
                        down = down - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= down - (Constants.SideLength + Constants.SplitterSpan);
                        down = Constants.SideLength + Constants.SplitterSpan;
                    }
                }
            }

            if (up > 0)
            {
                _topSideContent.Measure(new Size(availableSize.Width, up - Constants.SplitterSpan));
                _topSplitter.Measure(new Size(availableSize.Width, Constants.SplitterSpan));
            }
            else
            {
                _topSideContent.Measure(new Size());
                _topSplitter.Measure(new Size());
            }
            if (down > 0)
            {
                _bottomSideContent.Measure(new Size(availableSize.Width, down - Constants.SplitterSpan));
                _bottomSplitter.Measure(new Size(availableSize.Width, Constants.SplitterSpan));
            }
            else
            {
                _bottomSideContent.Measure(new Size());
                _bottomSplitter.Measure(new Size());
            }


            if (_leftSideContent.HasContent)
            {
                left += _leftSideContent.ContentSideLenght;
                left += Constants.SplitterSpan;
            }

            if (_rightSideContent.HasContent)
            {
                right += _rightSideContent.ContentSideLenght;
                right += Constants.SplitterSpan;
            }

            if (left + right + Constants.SideLength > availableSize.Width)
            {
                double delta = left + right + Constants.SideLength - availableSize.Width;
                if (left > 0)
                {
                    if (left - delta >= Constants.SideLength + Constants.SplitterSpan)
                    {
                        left = left - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= left - (Constants.SideLength + Constants.SplitterSpan);
                        left = Constants.SideLength + Constants.SplitterSpan;
                    }
                }
                if (delta > 0 && right > 0)
                {
                    if (right - delta >= Constants.SideLength + Constants.SplitterSpan)
                    {
                        right = right - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= right - (Constants.SideLength + Constants.SplitterSpan);
                        right = Constants.SideLength + Constants.SplitterSpan;
                    }
                }
            }

            if (left > 0)
            {
                _leftSideContent.Measure(new Size(left - Constants.SplitterSpan, Math.Max(availableSize.Height - up - down, 0)));
                _leftSplitter.Measure(new Size(Constants.SplitterSpan, Math.Max(availableSize.Height - up - down, 0)));
            }
            else
            {
                _leftSideContent.Measure(new Size());
                _leftSplitter.Measure(new Size());
            }
            if (right > 0)
            {
                _rightSideContent.Measure(new Size(right - Constants.SplitterSpan, Math.Max(availableSize.Height - up - down, 0)));
                _rightSplitter.Measure(new Size(Constants.SplitterSpan, Math.Max(availableSize.Height - up - down, 0)));
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

            if (_topSideContent.HasContent)
            {
                up += _topSideContent.ContentSideLenght;
                up += Constants.SplitterSpan;
            }

            if (_bottomSideContent.HasContent)
            {
                down += _bottomSideContent.ContentSideLenght;
                down += Constants.SplitterSpan;
            }

            if (up + down + Constants.SideLength > finalSize.Height)
            {
                double delta = up + down + Constants.SideLength - finalSize.Height;
                if (up > 0)
                {
                    if (up - delta >= Constants.SideLength + Constants.SplitterSpan)
                    {
                        up = up - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= up - (Constants.SideLength + Constants.SplitterSpan);
                        up = Constants.SideLength + Constants.SplitterSpan;
                    }
                }
                if (delta > 0 && down > 0)
                {
                    if (down - delta >= Constants.SideLength + Constants.SplitterSpan)
                    {
                        down = down - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= down - (Constants.SideLength + Constants.SplitterSpan);
                        down = Constants.SideLength + Constants.SplitterSpan;
                    }
                }
            }

            if (up > 0)
            {
                _topSideContent.Arrange(new Rect(0, 0, finalSize.Width, up - Constants.SplitterSpan));
                _topSplitter.Arrange(new Rect(0, up - Constants.SplitterSpan, finalSize.Width, Constants.SplitterSpan));
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
                    _bottomSideContent.Arrange(new Rect(0, up + Constants.SplitterSpan, finalSize.Width, down - Constants.SplitterSpan));
                    _bottomSplitter.Arrange(new Rect(0, up, finalSize.Width, Constants.SplitterSpan));
                }
                else
                {
                    _bottomSideContent.Arrange(new Rect(0, finalSize.Height - (down - Constants.SplitterSpan), finalSize.Width, down - Constants.SplitterSpan));
                    _bottomSplitter.Arrange(new Rect(0, finalSize.Height - down, finalSize.Width, Constants.SplitterSpan));
                }
            }
            else
            {
                _bottomSideContent.Arrange(new Rect());
                _bottomSplitter.Arrange(new Rect());
            }



            if (_leftSideContent.HasContent)
            {
                left += _leftSideContent.ContentSideLenght;
                left += Constants.SplitterSpan;
            }

            if (_rightSideContent.HasContent)
            {
                right += _rightSideContent.ContentSideLenght;
                right += Constants.SplitterSpan;
            }

            if (left + right + Constants.SideLength > finalSize.Width)
            {
                double delta = left + right + Constants.SideLength - finalSize.Width;
                if (left > 0)
                {
                    if (left - delta >= Constants.SideLength + Constants.SplitterSpan)
                    {
                        left = left - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= left - (Constants.SideLength + Constants.SplitterSpan);
                        left = Constants.SideLength + Constants.SplitterSpan;
                    }
                }
                if (delta > 0 && right > 0)
                {
                    if (right - delta >= Constants.SideLength + Constants.SplitterSpan)
                    {
                        right = right - delta;
                        delta = 0;
                    }
                    else
                    {
                        delta -= right - (Constants.SideLength + Constants.SplitterSpan);
                        right = Constants.SideLength + Constants.SplitterSpan;
                    }
                }
            }

            double height = Math.Max(finalSize.Height - up - down, 0);
            if (left > 0)
            {
                _leftSideContent.Arrange(new Rect(0, up, left - Constants.SplitterSpan, height));
                _leftSplitter.Arrange(new Rect(left - Constants.SplitterSpan, up, Constants.SplitterSpan, height));
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
                    _rightSideContent.Arrange(new Rect(left + Constants.SplitterSpan, up, right - Constants.SplitterSpan, height));
                    _rightSplitter.Arrange(new Rect(left, up, Constants.SplitterSpan, height));
                }
                else
                {
                    _rightSideContent.Arrange(new Rect(finalSize.Width - (right - Constants.SplitterSpan), up, right - Constants.SplitterSpan, height));
                    _rightSplitter.Arrange(new Rect(finalSize.Width - right, up, Constants.SplitterSpan, height));
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