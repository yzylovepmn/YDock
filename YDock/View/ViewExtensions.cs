using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace YDock.View
{
    internal static class ViewExtensions
    {
        public static Point PointToScreenDPI(this Visual visual, Point pt)
        {
            Point resultPt = visual.PointToScreen(pt);
            return TransformToDeviceDPI(visual, resultPt);
        }

        public static Point PointToScreenDPIWithoutFlowDirection(this FrameworkElement element, Point point)
        {
            if (FrameworkElement.GetFlowDirection(element) == FlowDirection.RightToLeft)
            {
                var actualSize = element.TransformActualSizeToAncestor();
                Point leftToRightPoint = new Point(
                    actualSize.Width - point.X,
                    point.Y);
                return element.PointToScreenDPI(leftToRightPoint);
            }

            return element.PointToScreenDPI(point);
        }



        public static Rect GetScreenArea(this FrameworkElement element)
        {
            var point = element.PointToScreenDPI(new Point());
            if (FrameworkElement.GetFlowDirection(element) == FlowDirection.RightToLeft)
            {
                var actualSize = element.TransformActualSizeToAncestor();
                Point leftToRightPoint = new Point(
                    actualSize.Width - point.X,
                    point.Y);
                return new Rect(leftToRightPoint,
                    actualSize);
            }

            return new Rect(point,
                element.TransformActualSizeToAncestor());
        }

        public static Point TransformToDeviceDPI(this Visual visual, Point pt)
        {
            Matrix m = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Point(pt.X / m.M11, pt.Y / m.M22);
        }

        public static Size TransformFromDeviceDPI(this Visual visual, Size size)
        {
            Matrix m = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Size(size.Width * m.M11, size.Height * m.M22);
        }

        public static Point TransformFromDeviceDPI(this Visual visual, Point pt)
        {
            Matrix m = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Point(pt.X * m.M11, pt.Y * m.M22);
        }

        public static bool CanTransform(this Visual visual)
        {
            return PresentationSource.FromVisual(visual) != null;
        }

        public static Size TransformActualSizeToAncestor(this FrameworkElement element)
        {
            if (PresentationSource.FromVisual(element) == null)
                return new Size(element.ActualWidth, element.ActualHeight);

            var parentWindow = PresentationSource.FromVisual(element).RootVisual;
            var transformToWindow = element.TransformToAncestor(parentWindow);
            return transformToWindow.TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight)).Size;
        }

        public static Size TransformSizeToAncestor(this FrameworkElement element, Size sizeToTransform)
        {
            if (PresentationSource.FromVisual(element) == null)
                return sizeToTransform;

            var parentWindow = PresentationSource.FromVisual(element).RootVisual;
            var transformToWindow = element.TransformToAncestor(parentWindow);
            return transformToWindow.TransformBounds(new Rect(0, 0, sizeToTransform.Width, sizeToTransform.Height)).Size;
        }

        public static GeneralTransform TansformToAncestor(this FrameworkElement element)
        {
            if (PresentationSource.FromVisual(element) == null)
                return new MatrixTransform(Matrix.Identity);

            var parentWindow = PresentationSource.FromVisual(element).RootVisual;
            return element.TransformToAncestor(parentWindow);
        }

        public static DependencyObject FindVisualTreeRoot(this DependencyObject initial)
        {
            DependencyObject current = initial;
            DependencyObject result = initial;

            while (current != null)
            {
                result = current;
                if (current is Visual || current is Visual3D)
                {
                    current = VisualTreeHelper.GetParent(current);
                }
                else
                {
                    current = LogicalTreeHelper.GetParent(current);
                }
            }

            return result;
        }

        public static int FindIndex(this IList<Rect> bounds, Point p)
        {
            for (int i = 0; i < bounds.Count; i++)
            {
                if (bounds[i].Contains(p))
                    return i;
            }
            return -1;
        }
    }
}