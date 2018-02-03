using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class LayoutDocumentGroupControl : BaseGroupControl
    {
        static LayoutDocumentGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentGroupControl), new FrameworkPropertyMetadata(typeof(LayoutDocumentGroupControl)));
            FocusableProperty.OverrideMetadata(typeof(LayoutDocumentGroupControl), new FrameworkPropertyMetadata(false));
        }

        public LayoutDocumentGroupControl(ILayoutGroup model) : base(model)
        {
        }
    }
}