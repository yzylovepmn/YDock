using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using YDock.View;

namespace YDock
{
    public class DockHelper
    {
        public static T GetTemplateChild<T>(FrameworkTemplate template, FrameworkElement templateParent, string name)
        {
            return (T)template.FindName(name, templateParent);
        }

        public static Window CreateTransparentWindow()
        {
            return new Window()
            {
                Height = SystemParameters.FullPrimaryScreenHeight,
                Width = SystemParameters.FullPrimaryScreenWidth,
                AllowsTransparency = true,
                Background = new SolidColorBrush(new Color() { A = 1 }),
                WindowStyle = WindowStyle.None,
                WindowState = WindowState.Maximized,
                ShowInTaskbar = false
            };
        }

        public static Rect CreateChildRectFromParent(FrameworkElement parent, FrameworkElement child)
        {
            var originP = parent.PointToScreenDPIWithoutFlowDirection(new Point());
            var childP = child.PointToScreenDPIWithoutFlowDirection(new Point());
            return new Rect(new Point(childP.X - originP.X, childP.Y - originP.Y), child.TransformActualSizeToAncestor());
        }

        public static Point GetMousePosition(FrameworkElement relativeTo)
        {
            var mouseP = Mouse.GetPosition(relativeTo);
            var pToScreen = relativeTo.PointToScreenDPIWithoutFlowDirection(new Point());
            return new Point(pToScreen.X + mouseP.X, pToScreen.Y + mouseP.Y);
        }
    }

    public class ElementComparer<T> : IComparer<T>
    {
        private Func<T, T, int> _comparer;

        public ElementComparer(Func<T, T, int> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer(x, y);
        }
    }

    public static class ResourceManager
    {
        public static readonly SolidColorBrush SplitterBrushVertical;
        public static readonly SolidColorBrush SplitterBrushHorizontal;
        public static readonly SolidColorBrush TextBlockActiveForeground;
        public static readonly Pen ActiveDashPen;
        public static readonly Pen DisActiveDashPen;

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

            TextBlockActiveForeground = new SolidColorBrush(new Color()
            {
                R = 0x00,
                G = 0x7A,
                B = 0xCC,
                A = 0xFF
            });

            ActiveDashPen = new Pen()
            {
                Brush = Brushes.White,
                Thickness = 0.8,
                DashCap = PenLineCap.Flat,
                DashStyle = new DashStyle(new List<double>() { 1, 4 }, 0)
            };

            DisActiveDashPen = new Pen()
            {
                Brush = Brushes.Black,
                Thickness = 0.8,
                DashCap = PenLineCap.Flat,
                DashStyle = new DashStyle(new List<double>() { 1, 4 }, 0)
            };
        }
    }
}