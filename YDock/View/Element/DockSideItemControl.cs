using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YDock.Model;

namespace YDock.View
{
    public class DockSideItemControl : Control
    {
        static DockSideItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockSideItemControl), new FrameworkPropertyMetadata(typeof(DockSideItemControl)));
            FocusableProperty.OverrideMetadata(typeof(DockSideItemControl), new FrameworkPropertyMetadata(false));
        }

        public DockSideItemControl() { }

        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(LayoutElement), typeof(DockSideItemControl));

        public LayoutElement Model
        {
            get { return (LayoutElement)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }
    }
}