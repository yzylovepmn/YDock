using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace YDock.View
{
    public class LayoutGridSplitter : Thumb
    {
        static LayoutGridSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutGridSplitter), new FrameworkPropertyMetadata(typeof(LayoutGridSplitter)));
            HorizontalAlignmentProperty.OverrideMetadata(typeof(LayoutGridSplitter), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            VerticalAlignmentProperty.OverrideMetadata(typeof(LayoutGridSplitter), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            BackgroundProperty.OverrideMetadata(typeof(LayoutGridSplitter), new FrameworkPropertyMetadata(Brushes.Transparent));
            IsHitTestVisibleProperty.OverrideMetadata(typeof(LayoutGridSplitter), new FrameworkPropertyMetadata(true, null));
        }

        public LayoutGridSplitter()
        {

        }
    }
}