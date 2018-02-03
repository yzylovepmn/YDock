using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class DragTabWindow : BaseFloatWindow
    {
        private DragTabWindow(DragItem dragItem) : base(true)
        {
            _widthEceeed = Constants.FloatWindowResizeLength * 2;
            _heightEceeed = Constants.FloatWindowResizeLength * 2;
            Background = Brushes.Transparent;
            ShowInTaskbar = false;
            _dragItem = dragItem;
        }

        private DragItem _dragItem;

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Content = null;

            if (NeedReCreate)
            {
                double top = Top, left = Left;
                var ele = _dragItem.RelativeObj as IDockElement;
                Border parent = (Border)VisualTreeHelper.GetParent(ele.Content);
                parent.Child = null;
                BaseFloatWindow floatWnd;
                ILayoutGroupControl groupCtrl;
                if (ele.Container is LayoutDocumentGroup)
                {
                    groupCtrl = new LayoutDocumentGroupControl(ele.Container);
                    floatWnd = new DocumentGroupWindow()
                    {
                        Top = top - Constants.FloatWindowResizeLength - Constants.FloatWindowHeaderHeight,
                        Left = left - Constants.FloatWindowResizeLength - Constants.DocumentWindowPadding
                    };
                }
                else
                {
                    groupCtrl = new AnchorSideGroupControl(ele.Container);
                    floatWnd = new SingleAnchorWindow()
                    {
                        Top = top - Constants.FloatWindowResizeLength,
                        Left = left - Constants.FloatWindowResizeLength
                    };
                }
                floatWnd.AttachChild(groupCtrl, 0);
                floatWnd.Show();
            }
            _dragItem = null;
        }

        internal static DragTabWindow CreateDragTabWindow(DragItem dragItem)
        {
            var ele = dragItem.RelativeObj as IDockElement;
            var _internelGrid = new Grid();
            var dragTabWindow = new DragTabWindow(dragItem) { Width = (ele.Content as FrameworkElement).ActualWidth + 2 };
            dragTabWindow.Content = _internelGrid;
            _internelGrid.RowDefinitions.Add(new RowDefinition());
            _internelGrid.RowDefinitions.Add(new RowDefinition());
            //创建内容的Border
            var border = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = ResourceManager.SplitterBrushHorizontal,
                SnapsToDevicePixels = true,
                ClipToBounds = true
            };
            border.Child = ele.Content;
            //创建封装Header的Canvas
            var canvas = new Canvas() { Background = Brushes.Transparent };
            _internelGrid.Children.Add(border);
            _internelGrid.Children.Add(canvas);
            if (ele.Container is LayoutDocumentGroup)
            {
                border.Background = Brushes.White;
                //Content 在第二行
                Grid.SetRow(border, 1);
                var header = new DragTabItem(null)
                {
                    Template = ele.DockManager.DocumentHeaderTemplate,
                    DataContext = ele,
                    //禁用命中测试
                    IsHitTestVisible = false
                };
                _internelGrid.RowDefinitions[0].Height = new GridLength(21, GridUnitType.Pixel);
                _internelGrid.RowDefinitions[1].Height = new GridLength((ele.Content as FrameworkElement).ActualHeight + 2);
                canvas.Children.Add(header);
            }
            else
            {
                border.Background = ResourceManager.SplitterBrushVertical;
                //header 在第二行
                Grid.SetRow(canvas, 1);

                border = new Border()
                {
                    Height = dragItem.ClickRect.Height,
                    Width = dragItem.ClickRect.Width,
                    BorderThickness = new Thickness(1, 0, 1, 1),
                    BorderBrush = ResourceManager.SplitterBrushHorizontal,
                    SnapsToDevicePixels = true,
                    ClipToBounds = true,
                    Padding = new Thickness(4, 2, 4, 2),
                    Background = ResourceManager.SplitterBrushVertical,
                    Margin = new Thickness(0,-0.8,0,0)
                };
                var textblock = new TextBlock()
                {
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Text = ele.Title,
                    Foreground = ResourceManager.TextBlockActiveForeground
                };
                border.Child = textblock;
                canvas.Children.Add(border);
                Canvas.SetLeft(border, dragItem.ClickRect.Left);
            }
            dragTabWindow.Owner = ele.DockManager.MainWindow;
            return dragTabWindow;
        }
    }
}