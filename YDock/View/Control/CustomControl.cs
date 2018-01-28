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

        public CustomToggleButton()
        {

        }

        public static readonly DependencyProperty DropContextMenuProperty =
            DependencyProperty.Register("DropContextMenu", typeof(CustomContextMenu), typeof(CustomToggleButton));


        public CustomContextMenu DropContextMenu
        {
            get { return (CustomContextMenu)GetValue(DropContextMenuProperty); }
            set { SetValue(DropContextMenuProperty, value); }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (DropContextMenu != null)
            {
                DropContextMenu.PlacementTarget = this;
                DropContextMenu.Placement = PlacementMode.Bottom;
                DropContextMenu.Closed += OnDropContextMenuClosed;
                DropContextMenu.IsOpen = true;
            }
            IsChecked = true;
        }

        private void OnDropContextMenuClosed(object sender, RoutedEventArgs e)
        {
            IsChecked = false;
            DropContextMenu.Closed -= OnDropContextMenuClosed;
        }

        public void Dispose()
        {
            DataContext = null;
            DropContextMenu = null;
        }
    }

    public class CustomContextMenu : ContextMenu
    {
        static CustomContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomContextMenu), new FrameworkPropertyMetadata(typeof(CustomContextMenu)));
        }

        protected override void OnOpened(RoutedEventArgs e)
        {
            BindingOperations.GetBindingExpression(this, ItemsSourceProperty).UpdateTarget();

            base.OnOpened(e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomMenuItem();
        }
    }

    public class CustomMenuItem : MenuItem
    {
        static CustomMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomMenuItem), new FrameworkPropertyMetadata(typeof(CustomMenuItem)));
        }

        protected override void OnClick()
        {
            base.OnClick();
            var ele = DataContext as IDockElement;
            if (ele.Container is LayoutDocumentGroup)
                (ele.Container as LayoutDocumentGroup).SetActive(ele);
        }
    }
}