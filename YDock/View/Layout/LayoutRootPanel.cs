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
    /// <summary>
    /// 用于容纳<see cref="LayoutGroupPanel"/>,以及AutoHideWindow
    /// </summary>
    public class LayoutRootPanel : Panel, IView
    {
        static LayoutRootPanel()
        {
            FocusableProperty.OverrideMetadata(typeof(LayoutRootPanel), new FrameworkPropertyMetadata(false));
        }

        public LayoutRootPanel(IModel model)
        {
            Model = model;
            RootGroupPanel = new LayoutGroupPanel() { ContainDocument = false, IsDocumentPanel = true };
            AHWindow = new AutoHideWindow();
        }

        private LayoutGroupPanel _rootGroupPanel;
        public LayoutGroupPanel RootGroupPanel
        {
            get { return _rootGroupPanel; }
            internal set
            {
                if (_rootGroupPanel != value)
                {
                    if (_rootGroupPanel != null)
                        Children.Remove(_rootGroupPanel);
                    _rootGroupPanel = value;
                    if (_rootGroupPanel != null)
                        Children.Add(_rootGroupPanel);
                }
            }
        }

        private AutoHideWindow _ahWindow;
        public AutoHideWindow AHWindow
        {
            get { return _ahWindow; }
            set
            {
                if (_ahWindow != value)
                {
                    if (_ahWindow != null)
                        Children.Remove(_ahWindow);
                    _ahWindow = value;
                    if (_ahWindow != null)
                    {
                        Children.Add(_ahWindow);
                        SetZIndex(_ahWindow, 2);
                    }
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _rootGroupPanel.Measure(new Size(availableSize.Width - 10, availableSize.Height - 10));
            switch (_ahWindow.Side)
            {
                case DockSide.Right:
                case DockSide.Left:
                    _ahWindow.Measure(new Size(availableSize.Width, availableSize.Height - 10));
                    break;
                case DockSide.Top:
                case DockSide.Bottom:
                    _ahWindow.Measure(new Size(availableSize.Width - 10, availableSize.Height));
                    break;
                default:
                    _ahWindow.Measure(availableSize);
                    break;
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _rootGroupPanel.Arrange(new Rect(new Point(5, 5), finalSize));
            switch (_ahWindow.Side)
            {
                case DockSide.Left:
                    _ahWindow.Arrange(new Rect(new Point(2, 5), _ahWindow.DesiredSize));
                    break;
                case DockSide.Right:
                    _ahWindow.Arrange(new Rect(new Point(finalSize.Width - _ahWindow.DesiredSize.Width - 2, 5), _ahWindow.DesiredSize));
                    break;
                case DockSide.Top:
                    _ahWindow.Arrange(new Rect(new Point(5, 2), _ahWindow.DesiredSize));
                    break;
                case DockSide.Bottom:
                    _ahWindow.Arrange(new Rect(new Point(5, finalSize.Height - _ahWindow.DesiredSize.Height - 2), _ahWindow.DesiredSize));
                    break;
                case DockSide.None:
                    _ahWindow.Arrange(new Rect());
                    break;
            }
            return finalSize;
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
    }
}