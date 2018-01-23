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
    public class DragTabItem : TabItem, IDisposable
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

        static ILayoutElement _dragItem;
        static IList<Rect> _childrenBounds;

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
            _dragItem = null;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (VisualParent == null) return;
            if (!IsMouseCaptured)
                CaptureMouse();
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
                if (VisualParent != null && _dragItem != null)
                {
                    var parent = VisualParent as Panel;
                    var p = e.GetPosition(parent);
                    int src = _container.IndexOf(_dragItem);
                    int des = _childrenBounds.FindIndex(p);
                    if (des < 0 && _dragItem != null)
                    {
                        _dragItem = null;
                        //TODO Drag
                    }
                    else
                    {
                        if (parent.Children[src].IsMouseCaptured)
                        {
                            if (src != des)
                                MoveTo(src, des, parent);
                        }
                        else
                        {
                            if (src == des)
                                parent.Children[src].CaptureMouse();
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
                                    else if(!parent.Children[des].IsMouseCaptured)
                                        parent.Children[des].CaptureMouse();
                                }
                                else
                                {
                                    double len = 0;
                                    for (int i = 0; i < src; i++)
                                        len += _childrenBounds[i].Size.Width;
                                    len += _childrenBounds[des].Size.Width;
                                    if (len < p.X)
                                        MoveTo(src, des, parent);
                                    else if (!parent.Children[des].IsMouseCaptured)
                                        parent.Children[des].CaptureMouse();
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