using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YDock.Model;
using YDock.View;

namespace YDockTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private Window wnd;
        //private bool isDragging = false;
        //private Matrix transform;
        //private Point delta;


        //private void UpdateLocation(double top, double left)
        //{
        //    if (wnd == null) return;
        //    wnd.Top = top;
        //    wnd.Left = left;
        //}

        //private T GetVisual<T>(Visual relativeObj, Point p) where T : UIElement
        //{
        //    var hittest = VisualTreeHelper.HitTest(relativeObj, p);
        //    if (hittest == null) return default(T);
        //    else
        //    {
        //        if (hittest.VisualHit is T) return (T)hittest.VisualHit;
        //        else return default(T);
        //    }
        //}

        //private Window CreateWnd(UIElement content)
        //{
        //    //((TextBlock)content).LayoutTransform = new RotateTransform(270);
        //    return new Window()
        //    {
        //        AllowsTransparency = true,
        //        Background = null,
        //        ResizeMode = ResizeMode.NoResize,
        //        WindowStyle = WindowStyle.None,
        //        ShowInTaskbar = false,
        //        Content = content,
        //        Owner = this,
        //        IsHitTestVisible = false,
        //        ShowActivated = false,
        //        SizeToContent = SizeToContent.WidthAndHeight
        //    };
        //}

        //public Point GetMousePosition()
        //{
        //    System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
        //    return new System.Windows.Point(point.X, point.Y);
        //}

        //private void OnMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        if (isDragging)
        //        {
        //            var p = transform.Transform(GetMousePosition());
        //            UpdateLocation(p.Y - delta.Y, p.X - delta.X);
        //        }
        //    }
        //}

        //private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var ele = GetVisual<TextBlock>(GD, e.GetPosition(GD));
        //    if (ele != null)
        //    {
        //        delta = e.GetPosition(ele);
        //        transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
        //        var p = transform.Transform(GetMousePosition());
        //        ((Grid)ele.Parent).Children.Remove(ele);
        //        wnd = CreateWnd(ele);
        //        UpdateLocation(p.Y - delta.Y, p.X - delta.X);
        //        wnd.Show();
        //        isDragging = true;
        //        GD.CaptureMouse();
        //    }
        //}

        //private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (wnd == null) return;
        //    var content = wnd.Content;
        //    wnd.Content = null;
        //    wnd.Close();
        //    wnd = null;
        //    GD.Children.Add((UIElement)content);
        //    GD.ReleaseMouseCapture();
        //    isDragging = false;
        //}
    }
}