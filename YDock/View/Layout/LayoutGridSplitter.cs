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

        #region BackgroundWhileDragging


        public static readonly DependencyProperty BackgroundWhileDraggingProperty =
            DependencyProperty.Register("BackgroundWhileDragging", typeof(Brush), typeof(LayoutGridSplitter),
                new FrameworkPropertyMetadata(Brushes.Black));


        public Brush BackgroundWhileDragging
        {
            get { return (Brush)GetValue(BackgroundWhileDraggingProperty); }
            set { SetValue(BackgroundWhileDraggingProperty, value); }
        }

        #endregion

        #region OpacityWhileDragging

        public static readonly DependencyProperty OpacityWhileDraggingProperty =
            DependencyProperty.Register("OpacityWhileDragging", typeof(double), typeof(LayoutGridSplitter),
                new FrameworkPropertyMetadata(0.5));

        public double OpacityWhileDragging
        {
            get { return (double)GetValue(OpacityWhileDraggingProperty); }
            set { SetValue(OpacityWhileDraggingProperty, value); }
        }

        #endregion
    }
}