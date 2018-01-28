using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock
{
    public class DockControl : IDockControl
    {
        internal DockControl(IDockElement prototype)
        {
            _prototype = prototype;
            (_prototype as DockElement).DockControl = this;
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

        public DockMode Mode
        {
            get
            {
                return _prototype.Mode;
            }
        }

        public bool IsVisible
        {
            get { return _prototype.IsVisible; }
        }

        public bool IsActive
        {
            get { return _prototype.IsActive; }
        }

        public bool CanSelect
        {
            get { return _prototype.CanSelect; }
        }

        public ILayoutGroup Container
        {
            get { return _prototype.Container; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        /// <summary>
        /// 通用显示的方法。
        /// 显示的模式（Dock，Float，AnchorSide）与当前Status有关，若需要切换显示模式请调用其它方法
        /// </summary>
        public void Show()
        {
            if (IsVisible) return;
            Container.SetActive(_prototype);
        }

        /// <summary>
        /// 以指定模式显示该项，若该项原模式与指定模式不同，则切换为指定模式
        /// </summary>
        /// <param name="mode"></param>
        public void ShowWithMode(DockMode mode)
        {
            if (mode == Mode)
                Show();
            switch (mode)
            {
                case DockMode.Normal:
                    break;
                case DockMode.DockBar:
                    break;
                case DockMode.Float:
                    break;
            }
        }

        /// <summary>
        /// 对于Normal or Float模式，此方法会直接从用户界面隐藏该项（CanSelect设为False）
        /// 对于DockBar模式：
        /// 若以自动隐藏窗口显示，则关闭窗口，但不从DockBar移除此项，用户依旧可以在界面选择（即CanSelect设为True）；
        /// 否则不做任何操作。若需要将该项从DockBar移除，需要调用Close方法
        /// </summary>
        public void Hide()
        {

        }

        /// <summary>
        /// 将CanSelect设为False，并从界面移除此项
        /// 对于Normal or Float模式，效果与Hide方法相同
        /// </summary>
        public void Close()
        {

        }

        public void Dispose()
        {
            _prototype.PropertyChanged -= PropertyChanged;
            _prototype.Dispose();
            _prototype = null;
        }
    }
}