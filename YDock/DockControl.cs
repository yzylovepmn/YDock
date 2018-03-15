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
using YDock.View;

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

        public bool IsDocument
        {
            get { return _prototype.IsDocument; }
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

        /// <summary>
        /// 是否可以转为浮动模式
        /// </summary>
        public bool CanFloat
        {
            get
            {
                return _prototype == null ? false : _prototype.CanFloat;
            }
        }

        /// <summary>
        /// 是否可以转为Dock模式
        /// </summary>
        public bool CanDock
        {
            get
            {
                return _prototype == null ? false : _prototype.CanDock;
            }
        }

        /// <summary>
        /// 是否可以转为Document模式
        /// </summary>
        public bool CanDockAsDocument
        {
            get
            {
                return _prototype == null ? false : _prototype.CanDockAsDocument;
            }
        }

        /// <summary>
        /// 是否可以切换自动隐藏状态
        /// </summary>
        public bool CanSwitchAutoHideStatus
        {
            get
            {
                return _prototype == null ? false : _prototype.CanSwitchAutoHideStatus;
            }
        }

        /// <summary>
        /// 是否可以隐藏
        /// </summary>
        public bool CanHide
        {
            get
            {
                return _prototype == null ? false : _prototype.CanHide;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        /// <summary>
        /// 通用显示的方法。
        /// 显示的模式（Dock，Float，AnchorSide）与当前Status有关
        /// </summary>
        public void Show()
        {
            if (IsVisible) return;
            Container.SetActive(_prototype);
        }

        /// <summary>
        /// 对于Normal or Float模式，此方法会直接从用户界面隐藏该项（CanSelect设为False）
        /// 对于DockBar模式：
        /// 若以自动隐藏窗口显示，则关闭窗口，但不从DockBar移除此项，用户依旧可以在界面选择（即CanSelect设为True）；
        /// 否则不做任何操作。若需要将该项从DockBar移除，需要调用Close方法
        /// </summary>
        public void Hide()
        {
            _prototype?.Hide();
        }

        /// <summary>
        /// 转为浮动窗口
        /// </summary>
        public void ToFloat()
        {
            _prototype?.ToFloat();
        }

        /// <summary>
        /// 转为Dock模式
        /// </summary>
        public void ToDock()
        {
            _prototype?.ToDock();
        }

        /// <summary>
        /// 转为Document模式
        /// </summary>
        public void ToDockAsDocument()
        {
            _prototype?.ToDockAsDocument();
        }

        /// <summary>
        /// 在Normal和DockBar模式间切换
        /// </summary>
        public void SwitchAutoHideStatus()
        {
            _prototype?.SwitchAutoHideStatus();
        }

        /// <summary>
        /// 将CanSelect设为False，并从界面移除此项
        /// 对于Normal or Float模式，效果与Hide方法相同
        /// </summary>
        public void Close()
        {
            Hide();
        }

        private bool _isDisposed = false;
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }
        public void Dispose()
        {
            if (_isDisposed) return;
            _prototype.PropertyChanged -= PropertyChanged;
            _prototype.Dispose();
            _prototype = null;
            _isDisposed = true;
        }
    }
}