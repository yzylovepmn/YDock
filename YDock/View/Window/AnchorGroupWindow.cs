using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YDock.Interface;

namespace YDock.View
{
    public class AnchorGroupWindow : BaseFloatWindow
    {
        static AnchorGroupWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorGroupWindow), new FrameworkPropertyMetadata(typeof(AnchorGroupWindow)));
        }

        public bool NoBorder
        {
            get
            {
                return Content != null && Content is BaseGroupControl
                    && (Content as BaseGroupControl).Items.Count == 1;
            }
        }


        public override void AttachChild(IDockView child, int index)
        {
            base.AttachChild(child, index);
            if (child is ILayoutPanel)
                Owner = (child as ILayoutPanel).DockManager.MainWindow;
            else Owner = child.Model.DockManager.MainWindow;
        }
    }
}