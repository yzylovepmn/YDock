using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace YDock
{
    public class YDockHelper
    {
        public static T GetTemplateChild<T>(FrameworkTemplate template, FrameworkElement templateParent, string name)
        {
            return (T)template.FindName(name, templateParent);
        }

        public static double GetMaxOrMinValue(double value, double min, double max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        public static bool IsSizeEmpty(Size size)
        {
            return size.Width < 0.1 && size.Height < 0.1;
        }

        public static bool IsSizeEmpty(double width, double height)
        {
            return width < 0.1 && height < 0.1;
        }
    }
}