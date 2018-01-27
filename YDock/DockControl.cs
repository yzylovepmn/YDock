using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;

namespace YDock
{
    public class DockControl : IDockControl
    {
        internal DockControl(IDockElement prototype)
        {
            _prototype = prototype;
            prototype.PropertyChanged += PropertyChanged;
        }

        #region ProtoType
        private IDockElement _prototype;

        public IDockElement Prototype
        {
            get { return _prototype; }
        }
        #endregion

        #region Interface
        public int ID
        {
            get
            {
                return _prototype.ID;
            }
        }

        public string Title
        {
            get
            {
                return _prototype.Title;
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                return _prototype.ImageSource;
            }
        }

        public UIElement Content
        {
            get
            {
                return _prototype.Content;
            }
        }

        public DockSide Side
        {
            get
            {
                return _prototype.Side;
            }
        }

        public DockManager DockManager
        {
            get
            {
                return _prototype.DockManager;
            }
        }

        public double DesiredWidth
        {
            get
            {
                return _prototype.DesiredWidth;
            }

            set
            {
                _prototype.DesiredWidth = value;
            }
        }

        public double DesiredHeight
        {
            get
            {
                return _prototype.DesiredHeight;
            }

            set
            {
                _prototype.DesiredHeight = value;
            }
        }

        public DockStatus Status
        {
            get
            {
                return _prototype.Status;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            _prototype.Dispose();
            _prototype = null;
        }
        #endregion
    }
}