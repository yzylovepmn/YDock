using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using YDock.Model;

namespace YDock.View
{
    public class AnchorSidePanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var visibleChildren = InternalChildren.Cast<FrameworkElement>().Where(ele => ele.Visibility != Visibility.Collapsed);

            double height = 0.0;
            double width = 0.0;
            foreach (var child in visibleChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                width += child.DesiredSize.Width + child.Margin.Left + child.Margin.Right;
                height = Math.Max(height, child.DesiredSize.Height);
            }

            if (width > availableSize.Width)
            {
                double avaWidth = availableSize.Width / visibleChildren.Count();
                var ele_deceed = visibleChildren.Where((e) => e.DesiredSize.Width + e.Margin.Left + e.Margin.Right <= avaWidth);
                int cnt = visibleChildren.Count() - ele_deceed.Count();
                double subWidth = ele_deceed.Sum((e) => e.DesiredSize.Width + e.Margin.Left + e.Margin.Right);
                double exceed_avaWidth = (availableSize.Width - subWidth) / cnt;
                foreach (var child in visibleChildren)
                {
                    if (child.DesiredSize.Width + child.Margin.Left + child.Margin.Right > avaWidth)
                        child.Measure(new Size(exceed_avaWidth - child.Margin.Left - child.Margin.Right, height));
                }
            }

            return new Size(Math.Min(width, availableSize.Width), height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var visibleChildren = InternalChildren.Cast<FrameworkElement>().Where(ele => ele.Visibility != Visibility.Collapsed);

            double wholeWidth = visibleChildren.Sum((e) => e.DesiredSize.Width + e.Margin.Left + e.Margin.Right);
            double avaWidth = finalSize.Width / visibleChildren.Count();
            if (wholeWidth > finalSize.Width)
            {
                var ele_deceed = visibleChildren.Where((e) => e.DesiredSize.Width + e.Margin.Left + e.Margin.Right <= avaWidth);
                int cnt = visibleChildren.Count() - ele_deceed.Count();
                double subWidth = ele_deceed.Sum((e) => e.DesiredSize.Width + e.Margin.Left + e.Margin.Right);
                double exceed_avaWidth = (finalSize.Width - subWidth) / cnt;

                double offset = 0.0;
                foreach (FrameworkElement child in visibleChildren)
                {
                    if (child.DesiredSize.Width + child.Margin.Left + child.Margin.Right <= avaWidth)
                    {
                        child.Arrange(new Rect(new Point(offset + child.Margin.Left, 0), child.DesiredSize));
                        offset += child.DesiredSize.Width + child.Margin.Left + child.Margin.Right;
                    }
                    else
                    {
                        child.Arrange(new Rect(new Point(offset + child.Margin.Left, 0), new Size(exceed_avaWidth - child.Margin.Left - child.Margin.Right, child.DesiredSize.Height)));
                        offset += exceed_avaWidth;
                    }
                }
            }
            else
            {
                double offset = 0.0;
                foreach (FrameworkElement child in visibleChildren)
                {
                    child.Arrange(new Rect(new Point(offset + child.Margin.Left, 0), child.DesiredSize));
                    offset += child.DesiredSize.Width + child.Margin.Left + child.Margin.Right;
                }
            }
            return finalSize;
        }
    }
}