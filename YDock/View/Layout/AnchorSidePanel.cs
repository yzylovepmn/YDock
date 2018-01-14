using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YDock.Enum;
using YDock.Model;

namespace YDock.View
{
    public class AnchorSidePanel : Panel, INotifyPropertyChanged
    {
        static AnchorSidePanel()
        {
            FocusableProperty.OverrideMetadata(typeof(AnchorSidePanel), new FrameworkPropertyMetadata(false));
        }

        public AnchorSidePanel(Direction direction)
        {
            Direction = direction;
            _InitChildren();
        }

        private void _InitChildren()
        {
            _normalDocument = new AnchorDocumentControl();
            _splitDocument = new AnchorDocumentControl();
            _splitter = new LayoutGridSplitter() { Cursor = Direction == Direction.LeftToRight ? Cursors.SizeWE : Cursors.SizeNS };
            _splitter.DragStarted += OnDragStarted;
            _splitter.DragDelta += _OnDragDelta;
            _splitter.DragCompleted += _OnDragCompleted;
            _normalDocument.PropertyChanged += _PropertyChanged;
            _splitDocument.PropertyChanged += _PropertyChanged;
            Children.Add(_normalDocument);
            Children.Add(_splitDocument);
            Children.Add(_splitter);
        }

        private void _PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private LayoutGridSplitter _splitter;

        private AnchorDocumentControl _normalDocument;
        public AnchorDocumentControl NormalDocument
        {
            get { return _normalDocument; }
        }

        private AnchorDocumentControl _splitDocument;
        public AnchorDocumentControl SplitDocument
        {
            get { return _splitDocument; }
        }

        public bool HasContent
        {
            get { return _normalDocument?.Model != null || _splitDocument?.Model != null; }
        }

        public bool IsFullMode
        {
            get { return _normalDocument?.Model != null && _splitDocument?.Model != null; }
        }

        private double _ratio = 0.5;
        public double Ratio
        {
            get { return _ratio; }
            internal set
            {
                if (_ratio != value)
                {
                    _ratio = value;
                    InvalidateMeasure();
                }
            }
        }

        private double _contentSideLenght = 0;
        public double ContentSideLenght
        {
            get
            {
                return _contentSideLenght;
            }
            set
            {
                if(_contentSideLenght != value)
                {
                    _contentSideLenght = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ContentSideLenght"));
                }
            }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Direction), typeof(AnchorSidePanel)
                , new FrameworkPropertyMetadata(OnDirectionPropertyChanged));

        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        private static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AnchorSidePanel).OnDirectionPropertyChanged(e);
        }

        public virtual void OnDirectionPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        #region Drag
        private Window _dragWnd;
        private Rectangle _dragRect;
        private Point _pToPanel;

        private void _OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            double delta;
            if (Direction == Direction.LeftToRight)
                delta = Canvas.GetLeft(_dragRect) - _pToPanel.X;
            else delta = Canvas.GetTop(_dragRect) - _pToPanel.Y;

            if (Direction == Direction.LeftToRight)
            {
                (_normalDocument.Model as LayoutElement).Width += delta;
                (_splitDocument.Model as LayoutElement).Width -= delta;
                Ratio = _normalDocument.Model.Width / (_normalDocument.Model.Width + _splitDocument.Model.Width);
            }
            else
            {
                (_normalDocument.Model as LayoutElement).Height += delta;
                (_splitDocument.Model as LayoutElement).Height -= delta;
                Ratio = _normalDocument.Model.Height / (_normalDocument.Model.Height + _splitDocument.Model.Height);
            }

            _DisposeDragWnd();
        }

        private void _OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Direction == Direction.LeftToRight)
                Canvas.SetLeft(_dragRect, YDockHelper.GetMaxOrMinValue(_pToPanel.X + e.HorizontalChange, Constants.SideLength, Math.Max(ActualWidth - Constants.SideLength - Constants.SplitterSpan, Constants.SideLength)));
            else Canvas.SetTop(_dragRect, YDockHelper.GetMaxOrMinValue(_pToPanel.Y + e.VerticalChange, Constants.SideLength, Math.Max(ActualHeight - Constants.SideLength - Constants.SplitterSpan, Constants.SideLength)));
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
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
            _pToPanel = transfrom.Transform(new Point(0, 0));

            Canvas.SetLeft(_dragRect, _pToPanel.X);
            Canvas.SetTop(_dragRect, _pToPanel.Y);

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
        #endregion

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            double delta1 = 0;
            double delta2 = 0;
            double resLen;
            if (Direction == Direction.LeftToRight)
                resLen = availableSize.Width - Constants.SplitterSpan;
            else resLen = availableSize.Height - Constants.SplitterSpan;

            if (_normalDocument.Model != null && _splitDocument.Model != null)
            {
                foreach (UIElement child in InternalChildren)
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                if (Direction == Direction.LeftToRight)
                {
                    //double _ratio = _normalDocument.Model.Width / (_normalDocument.Model.Width + _splitDocument.Model.Width);
                    if (_ratio > 0.5)
                    {
                        delta2 = Math.Max((1 - _ratio) * resLen, Constants.SideLength);
                        delta1 = Math.Max(resLen - delta2, Constants.SideLength);
                    }
                    else
                    {
                        delta1 = Math.Max(_ratio * resLen, Constants.SideLength);
                        delta2 = Math.Max(resLen - delta1, Constants.SideLength);
                    }
                    (_normalDocument.Model as LayoutElement).Width = delta1;
                    (_splitDocument.Model as LayoutElement).Width = delta2;
                    (_normalDocument.Model as LayoutElement).Height = availableSize.Height;
                    (_splitDocument.Model as LayoutElement).Height = availableSize.Height;
                }
                else
                {
                    //double _ratio = _normalDocument.Model.Height / (_normalDocument.Model.Height + _splitDocument.Model.Height);
                    if (_ratio > 0.5)
                    {
                        delta2 = Math.Max((1 - _ratio) * resLen, Constants.SideLength);
                        delta1 = Math.Max(resLen - delta2, Constants.SideLength);
                    }
                    else
                    {
                        delta1 = Math.Max(_ratio * resLen, Constants.SideLength);
                        delta2 = Math.Max(resLen - delta1, Constants.SideLength);
                    }
                    (_normalDocument.Model as LayoutElement).Height = delta1;
                    (_splitDocument.Model as LayoutElement).Height = delta2;
                    (_normalDocument.Model as LayoutElement).Width = availableSize.Width;
                    (_splitDocument.Model as LayoutElement).Width = availableSize.Width;
                }
            }
            else if (_normalDocument.Model != null || _splitDocument.Model != null)
            {
                var adc = _normalDocument.Model != null ? _normalDocument : _splitDocument;
                if (adc == _normalDocument)
                {
                    _normalDocument.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    _splitter.Measure(new Size());
                    _splitDocument.Measure(new Size());
                }
                else
                {
                    _normalDocument.Measure(new Size());
                    _splitter.Measure(new Size());
                    _splitDocument.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }

                (adc.Model as LayoutElement).Width = availableSize.Width;
                (adc.Model as LayoutElement).Height = availableSize.Height;
                if (Direction == Direction.LeftToRight)
                {
                    if (adc == _normalDocument)
                        delta1 = adc.Model.Width;
                    else delta2 = adc.Model.Width;
                }
                else
                {
                    if (adc == _normalDocument)
                        delta1 = adc.Model.Height;
                    else delta2 = adc.Model.Height;
                }
            }
            else
            {
                foreach (UIElement child in InternalChildren)
                    child.Measure(new Size());
            }

            if (_normalDocument.Model != null && _splitDocument.Model != null)
            {
                if (Direction == Direction.LeftToRight)
                {
                    _normalDocument.Measure(new Size(delta1, availableSize.Height));
                    _splitter.Measure(new Size(Constants.SplitterSpan, availableSize.Height));
                    _splitDocument.Measure(new Size(delta2, availableSize.Height));
                }
                else
                {
                    _normalDocument.Measure(new Size(availableSize.Width, delta1));
                    _splitter.Measure(new Size(availableSize.Width, Constants.SplitterSpan));
                    _splitDocument.Measure(new Size(availableSize.Width, delta2));
                }
            }
            else if (_normalDocument.Model != null || _splitDocument.Model != null)
            {
                var adc = _normalDocument.Model != null ? _normalDocument : _splitDocument;
                if (Direction == Direction.LeftToRight)
                    adc.Measure(new Size(adc == _normalDocument ? delta1 : delta2, availableSize.Height));
                else adc.Measure(new Size(availableSize.Width, adc == _normalDocument ? delta1 : delta2));
            }

            return availableSize;
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_normalDocument.Model != null && _splitDocument.Model != null)
            {
                if (Direction == Direction.LeftToRight)
                {
                    if (_normalDocument.Model.Width + _splitDocument.Model.Width + Constants.SplitterSpan > finalSize.Width)
                    {
                        double res = finalSize.Width;
                        if (_normalDocument.Model.Width <= res)
                        {
                            res -= _normalDocument.Model.Width;
                            _normalDocument.Arrange(new Rect(new Point(0, 0), new Size(_normalDocument.Model.Width, finalSize.Height)));
                            if (Constants.SplitterSpan <= res)
                            {
                                res -= Constants.SplitterSpan;
                                _splitter.Arrange(new Rect(new Point(_normalDocument.Model.Width, 0), new Size(Constants.SplitterSpan, finalSize.Height)));
                                _splitDocument.Arrange(new Rect(new Point(_normalDocument.Model.Width + Constants.SplitterSpan, 0), new Size(res, finalSize.Height)));
                            }
                            else
                            {
                                _splitter.Arrange(new Rect(new Point(_normalDocument.Model.Width, 0), new Size(res, finalSize.Height)));
                                _splitDocument.Arrange(new Rect());
                                res = 0;
                            }
                        }
                        else
                        {
                            _normalDocument.Arrange(new Rect(new Point(0, 0), new Size(res, finalSize.Height)));
                            _splitter.Arrange(new Rect());
                            _splitDocument.Arrange(new Rect());
                            res = 0;
                        }
                    }
                    else
                    {
                        _normalDocument.Arrange(new Rect(new Point(0, 0), new Size(_normalDocument.Model.Width, finalSize.Height)));
                        _splitter.Arrange(new Rect(new Point(_normalDocument.Model.Width, 0), new Size(Constants.SplitterSpan, finalSize.Height)));
                        _splitDocument.Arrange(new Rect(new Point(_normalDocument.Model.Width + Constants.SplitterSpan, 0), new Size(_splitDocument.Model.Width, finalSize.Height)));
                    }
                }
                else
                {
                    if (_normalDocument.Model.Height + _splitDocument.Model.Height + Constants.SplitterSpan > finalSize.Height)
                    {
                        double res = finalSize.Height;
                        if (_normalDocument.Model.Height <= res)
                        {
                            res -= _normalDocument.Model.Height;
                            _normalDocument.Arrange(new Rect(new Point(0, 0), new Size(finalSize.Width, _normalDocument.Model.Height)));
                            if (Constants.SplitterSpan <= res)
                            {
                                res -= Constants.SplitterSpan;
                                _splitter.Arrange(new Rect(new Point(0, _normalDocument.Model.Height), new Size(finalSize.Width, Constants.SplitterSpan)));
                                _splitDocument.Arrange(new Rect(new Point(0, _normalDocument.Model.Height + Constants.SplitterSpan), new Size(finalSize.Width, res)));
                            }
                            else
                            {
                                _splitter.Arrange(new Rect(new Point(0, _normalDocument.Model.Height), new Size(finalSize.Width, res)));
                                _splitDocument.Arrange(new Rect());
                                res = 0;
                            }
                        }
                        else
                        {
                            _normalDocument.Arrange(new Rect(new Point(0, 0), new Size(finalSize.Width, res)));
                            _splitter.Arrange(new Rect());
                            _splitDocument.Arrange(new Rect());
                            res = 0;
                        }
                    }
                    else
                    {
                        _normalDocument.Arrange(new Rect(new Point(0, 0), new Size(finalSize.Width, _normalDocument.Model.Height)));
                        _splitter.Arrange(new Rect(new Point(0, _normalDocument.Model.Height), new Size(finalSize.Width, Constants.SplitterSpan)));
                        _splitDocument.Arrange(new Rect(new Point(0, _normalDocument.Model.Height + Constants.SplitterSpan), new Size(finalSize.Width, _splitDocument.Model.Height)));
                    }
                }
            }
            else if (_normalDocument.Model != null || _splitDocument.Model != null)
            {
                var adc = _normalDocument.Model != null ? _normalDocument : _splitDocument;
                adc.Arrange(new Rect(new Point(), new Size(finalSize.Width, finalSize.Height)));
                if (adc == _normalDocument)
                    _splitDocument.Arrange(new Rect());
                else _normalDocument.Arrange(new Rect());
                _splitter.Arrange(new Rect());
            }
            else
            {
                _normalDocument.Arrange(new Rect());
                _splitter.Arrange(new Rect());
                _splitDocument.Arrange(new Rect());
            }

            return finalSize;
        }
        #endregion
    }
}