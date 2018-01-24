using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace YDock.View
{
    public class LayoutDragSplitter : Thumb
    {
        static LayoutDragSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDragSplitter), new FrameworkPropertyMetadata(typeof(LayoutDragSplitter)));
            BackgroundProperty.OverrideMetadata(typeof(LayoutDragSplitter), new FrameworkPropertyMetadata(Brushes.Transparent));
            IsHitTestVisibleProperty.OverrideMetadata(typeof(LayoutDragSplitter), new FrameworkPropertyMetadata(true, null));
        }
    }
}