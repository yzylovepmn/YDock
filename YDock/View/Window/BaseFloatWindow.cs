using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YDock.Interface;

namespace YDock.View
{
    public abstract class BaseFloatWindow : Window, ILayoutViewParent
    {
        public ILayoutViewWithSize Child
        {
            get
            {
                return Content == null ? null : Content as ILayoutViewWithSize;
            }
        }

        public bool IsSpiltMode
        {
            get
            {
                return Content != null && Content is ILayoutPanel;
            }
        }

        public DockManager DockManager
        {
            get
            {
                if (Content != null)
                {
                    if (Content is ILayoutPanel)
                        return (Content as LayoutGroupPanel).DockManager;
                    else return Child.Model.DockManager;
                }
                return null;
            }
        }

        public virtual void AttachChild(IDockView child, int index)
        {
            if (Content != child)
                Content = child;
        }

        public virtual void DetachChild(IDockView child)
        {
            if (child == Content)
                Content = null;
        }
    }
}