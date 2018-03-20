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
using YDock;
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

        private DockControl doc_0;
        private DockControl doc_1;
        private DockControl doc_2;
        private DockControl doc_3;
        private DockControl left;
        private DockControl right;
        private DockControl top;
        private DockControl bottom;
        private DockControl left_1;
        private DockControl right_1;
        private DockControl top_1;
        private DockControl bottom_1;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            doc_0 = DockManager.RegisterDocument("Document_0", new TextBlock() { Text = "Document_Content0" });
            doc_1 = DockManager.RegisterDocument("Document_1", new TextBlock() { Text = "Document_Content1" });
            doc_2 = DockManager.RegisterDocument("Document_2", new TextBlock() { Text = "Document_Content2" });
            doc_3 = DockManager.RegisterDocument("Document_3", new TextBlock() { Text = "Document_Content3" });
            left = DockManager.RegisterDock("Dock_Left", new TextBlock() { Text = "Dock_Left_Content" });
            right = DockManager.RegisterDock("Dock_Right", new TextBlock() { Text = "Dock_Right_Content" }, null, DockSide.Right);
            top = DockManager.RegisterDock("Dock_Top", new TextBlock() { Text = "Dock_Top_Content" }, null, DockSide.Top);
            bottom = DockManager.RegisterDock("Dock_Bottom", new TextBlock() { Text = "Dock_Bottom_1_Content" }, null, DockSide.Bottom);
            left_1 = DockManager.RegisterDock("Dock_Left_1", new TextBlock() { Text = "Dock_Left_1_Content" });
            right_1 = DockManager.RegisterDock("Dock_Right_1", new TextBlock() { Text = "Dock_Righ_1t_Content" }, null, DockSide.Right);
            top_1 = DockManager.RegisterDock("Dock_Top_1", new TextBlock() { Text = "Dock_Top_1_Content" }, null, DockSide.Top);
            bottom_1 = DockManager.RegisterFloat("Dock_Bottom_1", new TextBlock() { Text = "Dock_Bottom_1_Content" }, null, DockSide.Bottom);
            doc_0.Show();
            doc_1.Show();
            doc_2.Show();
            doc_3.Show();
            left.Show();
            bottom.Show();
            right.Show();
            top.Show();
            left_1.Show();
            bottom_1.Show();
            right_1.Show();
            top_1.Show();
        }
    }
}