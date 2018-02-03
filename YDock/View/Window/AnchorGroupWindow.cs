using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using YDock.Interface;

namespace YDock.View
{
    /// <summary>
    /// Without WindowHeader
    /// </summary>
    public class SingleAnchorWindow : BaseFloatWindow
    {
        static SingleAnchorWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SingleAnchorWindow), new FrameworkPropertyMetadata(typeof(SingleAnchorWindow)));
        }

        public SingleAnchorWindow(bool needReCreate = false) : base(needReCreate)
        {
            ShowInTaskbar = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child">为ILayoutGroupControl类型</param>
        /// <param name="index"></param>
        public override void AttachChild(IDockView child, int index)
        {
            base.AttachChild(child, index);
            Owner = child.Model.DockManager.MainWindow;
        }
    }

    public class AnchorGroupWindow : BaseFloatWindow
    {
        static AnchorGroupWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorGroupWindow), new FrameworkPropertyMetadata(typeof(AnchorGroupWindow)));
        }

        public AnchorGroupWindow()
        {
            ShowInTaskbar = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child">为ILayoutPanel类型</param>
        /// <param name="index"></param>
        public override void AttachChild(IDockView child, int index)
        {
            _heightEceeed += Constants.FloatWindowHeaderHeight;
            base.AttachChild(child, index);
            Owner = (child as ILayoutPanel).DockManager.MainWindow;
        }
    }
}