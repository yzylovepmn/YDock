using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YDock.Interface;

namespace YDock.View
{
    public class LayoutHeaderControl : Control
    {
        static LayoutHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutHeaderControl), new FrameworkPropertyMetadata(default(LayoutHeaderControl)));
        }
    }
}