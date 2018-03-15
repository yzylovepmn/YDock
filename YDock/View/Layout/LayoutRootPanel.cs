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
    public class LayoutRootPanel : Panel, IDockView, ILayoutViewParent
    {
        static LayoutRootPanel()
        {
            FocusableProperty.OverrideMetadata(typeof(LayoutRootPanel), new FrameworkPropertyMetadata(false));
        }

        public LayoutRootPanel(IDockModel model)
        {
            Model = model;
            _InitContent();
        }

        private void _InitContent()
        {
            AHWindow = new AutoHideWindow();
            //先初始化Document区域
            RootGroupPanel = new LayoutGroupDocumentPanel();
            (_model as DockRoot).DocumentModel = new LayoutDocumentGroup(DockMode.Normal, DockViewParent as DockManager);
            var _documentControl = new LayoutDocumentGroupControl((_model as DockRoot).DocumentModel);
            RootGroupPanel._AttachChild(_documentControl, 0);
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
                    {
                        Children.Remove(_rootGroupPanel);
                        _rootGroupPanel.CloseDropWindow();
                    }
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
                    {
                        Children.Remove(_ahWindow);
                        _ahWindow.Dispose();
                    }
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
            _rootGroupPanel.Arrange(new Rect(new Point(5, 5), new Size(finalSize.Width - 10, finalSize.Height - 10)));
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

        private IDockModel _model;
        public IDockModel Model
        {
            get
            {
                return _model;
            }
            internal set
            {
                if (_model != value)
                {
                    if (_model != null)
                        (_model as DockRoot).View = null;
                    _model = value;
                    if (_model != null)
                        (_model as DockRoot).View = this;
                }
            }
        }

        public IDockView DockViewParent
        {
            get
            {
                return _model.DockManager;
            }
        }

        internal IDockView FindChildByLevel(int level, DockSide side)
        {
            IDockView view = _rootGroupPanel;
            while(level > 0)
            {
                level--;
                if (view is BaseGroupControl)
                    break;
                if (view is LayoutGroupPanel)
                {
                    var panel = (view as LayoutGroupPanel);
                    if (panel.Direction == Direction.None)
                        break;
                    if (panel.Direction == Direction.LeftToRight)
                    {
                        if (side == DockSide.Top || side == DockSide.Bottom)
                            break;
                        if (side == DockSide.Left)
                        {
                            var child = panel.Children[0];
                            if (child is LayoutDocumentGroupControl)
                                break;
                            view = child as IDockView;
                        }
                        if (side == DockSide.Right)
                        {
                            var child = panel.Children[panel.Count - 1];
                            if (child is LayoutDocumentGroupControl)
                                break;
                            view = child as IDockView;
                        }
                    }
                    else
                    {
                        if (side == DockSide.Left || side == DockSide.Right)
                            break;
                        if (side == DockSide.Top)
                        {
                            var child = panel.Children[0];
                            if (child is LayoutDocumentGroupControl)
                                break;
                            view = child as IDockView;
                        }
                        if (side == DockSide.Bottom)
                        {
                            var child = panel.Children[panel.Count - 1];
                            if (child is LayoutDocumentGroupControl)
                                break;
                            view = child as IDockView;
                        }
                    }
                }
            }
            return view;
        }

        public void AttachChild(IDockView child, AttachMode mode, int index)
        {
            if (child is LayoutGroupPanel)
                RootGroupPanel = child as LayoutGroupPanel;
        }

        public void DetachChild(IDockView child, bool force = true)
        {
            if (child == RootGroupPanel)
                RootGroupPanel = null;
        }

        public int IndexOf(IDockView child)
        {
            if (child == RootGroupPanel)
                return 0;
            else return -1;
        }

        public void Dispose()
        {
            Model = null;
            RootGroupPanel.Dispose();
            RootGroupPanel = null;
            AHWindow = null;
            Children.Clear();
        }
    }
}