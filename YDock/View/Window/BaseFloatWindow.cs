using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using YDock.Interface;

namespace YDock.View
{
    public abstract class BaseFloatWindow : Window, ILayoutViewParent
    {
        protected BaseFloatWindow(bool needReCreate = false)
        {
            MinWidth = 150;
            MinHeight = 60;
            _widthEceeed = Constants.FloatWindowResizeLength * 2;
            _heightEceeed = Constants.FloatWindowResizeLength * 2;
            _needReCreate = needReCreate;
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;
            ShowActivated = true;
        }

        protected double _widthEceeed;
        internal double WidthEceeed
        {
            get { return _widthEceeed; }
        }

        protected double _heightEceeed;
        internal double HeightEceeed
        {
            get { return _heightEceeed; }
        }

        internal ILayoutViewWithSize Child
        {
            get
            {
                return Content == null ? null : Content as ILayoutViewWithSize;
            }
        }

        private bool _needReCreate;
        internal bool NeedReCreate
        {
            get { return _needReCreate; }
            set { _needReCreate = value; }
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
            {
                Content = child;
                Height = (child as ILayoutSize).DesiredHeight + _heightEceeed;
                Width = (child as ILayoutSize).DesiredWidth + _widthEceeed;
            }
        }

        public virtual void DetachChild(IDockView child)
        {
            if (child == Content)
            {
                Content = null;
                Close();
            }
        }
    }
}