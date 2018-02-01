using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private bool _noBorder = false;
        public bool NoBorder
        {
            get
            {
                return _noBorder;
            }
            set
            {
                if (_noBorder != value)
                {
                    _noBorder = value;
                    RaisePropertyChanged("NoBorder");
                }
            }
        }


        public override void AttachChild(IDockView child, int index)
        {
            base.AttachChild(child, index);
            _UpdateNoBorder();
            if (child is ILayoutPanel)
                Owner = (child as ILayoutPanel).DockManager.MainWindow;
            else Owner = child.Model.DockManager.MainWindow;
        }

        public override void DetachChild(IDockView child)
        {
            base.DetachChild(child);
            _UpdateNoBorder();
        }

        void _UpdateNoBorder()
        {
            if (Content != null && Content is ILayoutGroupControl
                && (Content as BaseGroupControl).Items.Count == 1)
                NoBorder = true;
            else NoBorder = false;
        }
    }
}