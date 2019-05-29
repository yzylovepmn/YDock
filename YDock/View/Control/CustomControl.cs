using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class CustomToggleButton : ToggleButton, IDisposable
    {
        static CustomToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomToggleButton), new FrameworkPropertyMetadata(typeof(CustomToggleButton)));
        }

        internal CustomToggleButton()
        {

        }

        public static readonly DependencyProperty DropContextMenuProperty =
            DependencyProperty.Register("DropContextMenu", typeof(OnItemClickMenu), typeof(CustomToggleButton));


        public OnItemClickMenu DropContextMenu
        {
            get { return (OnItemClickMenu)GetValue(DropContextMenuProperty); }
            set { SetValue(DropContextMenuProperty, value); }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (DropContextMenu != null)
            {
                DropContextMenu.PlacementTarget = this;
                DropContextMenu.Placement = PlacementMode.Bottom;
                DropContextMenu.IsOpen = true;
            }
        }

        public void Dispose()
        {
            DataContext = null;
            DropContextMenu = null;
        }
    }

    public class CustomStyleMenu : ContextMenu
    {

    }

    public class OnItemClickMenu : CustomStyleMenu
    {
        protected override void OnOpened(RoutedEventArgs e)
        {
            BindingOperations.GetBindingExpression(this, ItemsSourceProperty).UpdateTarget();

            base.OnOpened(e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ClickMenuItem();
        }
    }

    public class ClickMenuItem : MenuItem
    {
        public ClickMenuItem()
        {
            SetBinding(HeaderProperty, new Binding("Title"));
            var image = new Image();
            Icon = image;
            image.SetBinding(Image.SourceProperty, new Binding("ImageSource"));
        }

        protected override void OnClick()
        {
            base.OnClick();
            var ele = DataContext as IDockElement;
            if (ele.Container is ILayoutGroup)
                (ele.Container as ILayoutGroup).ShowWithActive(ele);
        }
    }
}