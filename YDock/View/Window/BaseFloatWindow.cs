using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using YDock.Interface;

namespace YDock.View
{
    public abstract class BaseFloatWindow : Window, ILayoutViewParent, INotifyPropertyChanged
    {
        public ILayoutViewWithSize Child
        {
            get
            {
                return Content == null ? null : Content as ILayoutViewWithSize;
            }
        }

        private bool _isSpiltMode;
        public bool IsSpiltMode
        {
            get
            {
                return _isSpiltMode;
            }
            set
            {
                if (_isSpiltMode != value)
                {
                    _isSpiltMode = value;
                    RaisePropertyChanged("IsSpiltMode");
                }
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void AttachChild(IDockView child, int index)
        {
            if (Content != child)
                Content = child;
            _UpdateSpiltMode();
        }

        public virtual void DetachChild(IDockView child)
        {
            if (child == Content)
                Content = null;
            _UpdateSpiltMode();
        }

        void _UpdateSpiltMode()
        {
            if (Content != null && Content is ILayoutPanel)
                IsSpiltMode = true;
            else IsSpiltMode = false;
        }
    }
}