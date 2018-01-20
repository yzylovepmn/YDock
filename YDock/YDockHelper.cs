using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace YDock
{
    public class YDockHelper
    {
        public static T GetTemplateChild<T>(FrameworkTemplate template, FrameworkElement templateParent, string name)
        {
            return (T)template.FindName(name, templateParent);
        }
    }

    public static class ResourceManager
    {
        public static readonly SolidColorBrush SplitterBrushVertical;
        public static readonly SolidColorBrush SplitterBrushHorizontal;

        static ResourceManager()
        {
            SplitterBrushVertical = new SolidColorBrush(new Color()
            {
                R = 0xF6,
                G = 0xF6,
                B = 0xF6,
                A = 0xFF
            });

            SplitterBrushHorizontal = new SolidColorBrush(new Color()
            {
                R = 0xCC,
                G = 0xCE,
                B = 0xDB,
                A = 0xFF
            });
        }
    }
}