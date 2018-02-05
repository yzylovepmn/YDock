using Microsoft.Windows.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using YDock.Enum;
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

        public SingleAnchorWindow()
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

        protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32Helper.WM_NCLBUTTONDOWN:
                    if (Content != null)
                        ((Content as ILayoutGroupControl).Model as ILayoutGroup).SetActive(0);
                    break;
            }
            return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
        }

        public override void Recreate()
        {
            base.Recreate();
            if (_needReCreate)
            {
                _needReCreate = false;
                var layoutCtrl = Child as BaseGroupControl;
                layoutCtrl.IsDraggingFromDock = false;
            }
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