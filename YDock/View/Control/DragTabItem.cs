using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class DragTabItem : TabItem, IDockView, IDisposable
    {
        static DragTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragTabItem), new FrameworkPropertyMetadata(typeof(DragTabItem)));
        }

        public DragTabItem(ILayoutGroup container)
        {
            _container = container;
        }

        private ILayoutGroup _container;
        public ILayoutGroup Container
        {
            get
            {
                return _container;
            }

            set
            {
                if (_container != value)
                    _container = value;
            }
        }

        public IDockModel Model
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IDockView DockViewParent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        static ILayoutElement _dragItem;
        static IList<Rect> _childrenBounds;
        static bool _mouseInside = true;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if ((Content as LayoutElement).IsActive)
                    (Content as LayoutElement).DockManager.ActiveElement = null;
                (Content as LayoutElement).CanSelect = false;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
            _mouseInside = false;
            _dragItem = null;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured)
                CaptureMouse();
            _mouseInside = true;
            _dragItem = Content as ILayoutElement;
            UpdateChildrenBounds(VisualParent as Panel);

            base.OnMouseLeftButtonDown(e);
            //在基类事件处理后再设置
            _dragItem.DockManager.ActiveElement = _dragItem as LayoutElement;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_dragItem != null)
                {
                    var parent = VisualParent as Panel;
                    var p = e.GetPosition(parent);
                    int src = _container.IndexOf(_dragItem);
                    int des = _childrenBounds.FindIndex(p);
                    if (des < 0)
                    {
                        //_dragItem = null;
                        //TODO Drag
                    }
                    else
                    {
                        if (_mouseInside)
                        {
                            if (src != des)
                            {
                                MoveTo(src, des, parent);
                                _mouseInside = false;
                            }
                            else if (!_mouseInside)
                                _mouseInside = true;
                        }
                        else
                        {
                            if (src == des)
                                _mouseInside = true;
                            else
                            {
                                if (des < src)
                                {
                                    double len = 0;
                                    for (int i = 0; i < des; i++)
                                        len += _childrenBounds[i].Size.Width;
                                    len += _childrenBounds[src].Size.Width;
                                    if (len > p.X)
                                        MoveTo(src, des, parent);
                                }
                                else
                                {
                                    double len = 0;
                                    for (int i = 0; i < src; i++)
                                        len += _childrenBounds[i].Size.Width;
                                    len += _childrenBounds[des].Size.Width;
                                    if (len < p.X)
                                        MoveTo(src, des, parent);
                                }
                            }
                        }
                    }
                }
            }

            base.OnMouseMove(e);
        }

        public void Dispose()
        {
            _container = null;
        }

        private void MoveTo(int src, int des, Panel parent)
        {
            _container.MoveTo(src, des);
            (_container.View as TabControl).SelectedIndex = des;
            parent.UpdateLayout();
            parent.Children[des].CaptureMouse();
            UpdateChildrenBounds(parent);
        }

        private void UpdateChildrenBounds(Panel parent)
        {
            _childrenBounds = new List<Rect>();
            var originP = parent.PointToScreenDPIWithoutFlowDirection(new Point());
            foreach (TabItem child in parent.Children)
            {
                var childP = child.PointToScreenDPIWithoutFlowDirection(new Point());
                _childrenBounds.Add(new Rect(new Point(childP.X - originP.X, childP.Y - originP.Y), child.TransformActualSizeToAncestor()));
            }
        }
    }
}