using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class DragTabItem : TabItem, IDockView
    {
        static DragTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragTabItem), new FrameworkPropertyMetadata(typeof(DragTabItem)));
        }

        public DragTabItem(BaseGroupControl dockViewParent)
        {
            _dockViewParent = dockViewParent;
        }

        public ILayoutGroup Container
        {
            get
            {
                return _dockViewParent.Model as ILayoutGroup;
            }
        }

        public IDockModel Model
        {
            get
            {
                return null;
            }
        }

        private BaseGroupControl _dockViewParent;
        public IDockView DockViewParent
        {
            get
            {
                return _dockViewParent;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if ((Content as DockElement).IsActive)
                    Container.SetActive(null);
                (Content as DockElement).CanSelect = false;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (IsMouseCaptured)
                ReleaseMouseCapture();
            _dockViewParent._mouseInside = false;
            _dockViewParent._dragItem = null;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //在基类事件处理后再设置
            base.OnMouseLeftButtonDown(e);

            _dockViewParent._mouseInside = true;
            _dockViewParent._mouseDown = e.GetPosition(this);
            _dockViewParent._dragItem = Content as IDockElement;
            if (_dockViewParent._dragItem.Container is LayoutDocumentGroup)
                _dockViewParent._rect = new Rect();
            else _dockViewParent._rect = DockHelper.CreateChildRectFromParent(VisualParent as Panel, this);
            _dockViewParent.UpdateChildrenBounds(VisualParent as Panel);

            Container.SetActive(_dockViewParent._dragItem);

            if (!IsMouseCaptured)
                CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
            {
                if (_dockViewParent._dragItem != null)
                {
                    var parent = VisualParent as Panel;
                    var p = e.GetPosition(parent);
                    int src = Container.IndexOf(_dockViewParent._dragItem);
                    int des = _dockViewParent._childrenBounds.FindIndex(p);
                    if (des < 0)
                    {
                        if (IsMouseCaptured)
                            ReleaseMouseCapture();
                        //TODO Drag
                        var item = _dockViewParent._dragItem;
                        _dockViewParent._dragItem = null;
                        if (_dockViewParent is AnchorSideGroupControl)
                            _dockViewParent.Model.DockManager.DragManager.IntoDragAction(new DragItem(item, item.Mode, DragMode.Anchor, _dockViewParent._mouseDown, _dockViewParent._rect, new Size(item.DesiredWidth, item.DesiredHeight)));
                        else _dockViewParent.Model.DockManager.DragManager.IntoDragAction(new DragItem(item, item.Mode, DragMode.Document, _dockViewParent._mouseDown, _dockViewParent._rect, new Size(item.DesiredWidth, item.DesiredHeight)));
                    }
                    else
                    {
                        if (_dockViewParent._mouseInside)
                        {
                            if (src != des)
                            {
                                MoveTo(src, des, parent);
                                _dockViewParent._mouseInside = false;
                            }
                            else if (!_dockViewParent._mouseInside)
                                _dockViewParent._mouseInside = true;
                        }
                        else
                        {
                            if (src == des)
                                _dockViewParent._mouseInside = true;
                            else
                            {
                                if (des < src)
                                {
                                    double len = 0;
                                    for (int i = 0; i < des; i++)
                                        len += _dockViewParent._childrenBounds[i].Size.Width;
                                    len += _dockViewParent._childrenBounds[src].Size.Width;
                                    if (len > p.X)
                                        MoveTo(src, des, parent);
                                }
                                else
                                {
                                    double len = 0;
                                    for (int i = 0; i < src; i++)
                                        len += _dockViewParent._childrenBounds[i].Size.Width;
                                    len += _dockViewParent._childrenBounds[des].Size.Width;
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

        private void MoveTo(int src, int des, Panel parent)
        {
            Container.MoveTo(src, des);
            parent.UpdateLayout();
            (_dockViewParent as BaseGroupControl).SelectedIndex = des;
            parent.Children[des].CaptureMouse();
            _dockViewParent.UpdateChildrenBounds(parent);
        }

        public void Dispose()
        {
            _dockViewParent = null;
            Content = null;
        }
    }
}