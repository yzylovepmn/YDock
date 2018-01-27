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
using YDock.Enum;
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
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //DockManager.AddDocument(new LayoutElement() { Side = DockSide.None, Title = "Document", DesiredWidth = 10, Content = new TextBlock() { Text = "Content", VerticalAlignment = VerticalAlignment.Center } });
            //DockManager.AddAnchorChild(new LayoutElement() { Side = DockSide.Left, Title = "Document_Left", DesiredWidth = 200, Content = new TextBlock() { Text = "Content", VerticalAlignment = VerticalAlignment.Center } });
            //DockManager.AddAnchorChild(new LayoutElement() { Side = DockSide.Left, Title = "Document_Left", DesiredWidth = 200, Content = new TextBlock() { Text = "Content", VerticalAlignment = VerticalAlignment.Center } });
            //DockManager.AddAnchorChild(new LayoutElement() { Side = DockSide.Top, Title = "Document_Top", DesiredHeight = 180, Content = new TextBlock() { Text = "Content", VerticalAlignment = VerticalAlignment.Center } });
            //DockManager.AddAnchorChild(new LayoutElement() { Side = DockSide.Right, Title = "Document_Right", DesiredWidth = 200, Content = new TextBlock() { Text = "Content", VerticalAlignment = VerticalAlignment.Center } });
            //DockManager.AddAnchorChild(new LayoutElement() { Side = DockSide.Bottom, Title = "Document_Bottom", DesiredHeight = 180, Content = new TextBlock() { Text = "Content", VerticalAlignment = VerticalAlignment.Center } });
            //DockManager.AddSidePanel(DockSide.Left);
            DockManager.RegisterDocument("Document_0", new TextBlock() { Text = "Document_Content0" });
            DockManager.RegisterDocument("Document_1", new TextBlock() { Text = "Document_Content1" });
            DockManager.RegisterDock("Dock_Left", new TextBlock() { Text = "Dock_Left_Content" });
            DockManager.RegisterDock("Dock_Right", new TextBlock() { Text = "Dock_Right_Content" }, null, DockSide.Right);
            DockManager.RegisterDock("Dock_Top", new TextBlock() { Text = "Dock_Top_Content" }, null, DockSide.Top);
            DockManager.RegisterDock("Dock_Bottom", new TextBlock() { Text = "Dock_Bottom_Content" }, null, DockSide.Bottom);
        }
    }
}