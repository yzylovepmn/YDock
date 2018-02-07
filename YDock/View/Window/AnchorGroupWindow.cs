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
    public class AnchorGroupWindow : BaseFloatWindow, INotifyPropertyChanged
    {
        static AnchorGroupWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorGroupWindow), new FrameworkPropertyMetadata(typeof(AnchorGroupWindow)));
        }

        public AnchorGroupWindow()
        {
            ShowInTaskbar = false;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// 是否Content为<see cref="ILayoutGroupControl"/>
        /// </summary>
        public bool IsSingleMode
        {
            get
            {
                return Content != null && Content is ILayoutGroupControl;
            }
        }

        /// <summary>
        /// 是否需要Border
        /// </summary>
        public bool NoBorder
        {
            get
            {
                return IsSingleMode && (Content as BaseGroupControl).Items.Count == 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child">为ILayoutPanel类型</param>
        /// <param name="index"></param>
        public override void AttachChild(IDockView child, int index)
        {
            if (child is ILayoutPanel)
            {
                _heightEceeed += Constants.FloatWindowHeaderHeight;
                Owner = (child as ILayoutPanel).DockManager.MainWindow;
            }
            else Owner = (child as ILayoutGroupControl).Model.DockManager.MainWindow;
            base.AttachChild(child, index);
        }

        public override void DetachChild(IDockView child)
        {
            base.DetachChild(child);
            UpdateSize();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (newContent != null)
                UpdateTemplate();
        }

        protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32Helper.WM_NCLBUTTONDOWN:
                    if (IsSingleMode)
                        ((Content as ILayoutGroupControl).Model as ILayoutGroup).SetActive((Content as BaseGroupControl).SelectedIndex);
                    break;
                case Win32Helper.WM_NCRBUTTONDOWN:
                    if (IsSingleMode)
                        ((Content as ILayoutGroupControl).Model as ILayoutGroup).SetActive((Content as BaseGroupControl).SelectedIndex);
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

        public void UpdateTemplate()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("IsSingleMode"));
            PropertyChanged(this, new PropertyChangedEventArgs("HasBorder"));
        }

        public void UpdateSize()
        {
            if (IsSingleMode)
                _heightEceeed = Constants.FloatWindowResizeLength * 2;
            else _heightEceeed = Constants.FloatWindowResizeLength * 2 + Constants.FloatWindowHeaderHeight;
        }
    }
}