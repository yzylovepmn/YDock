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
            _protoType = prototype;
            (_protoType as DockElement).DockControl = this;
            prototype.PropertyChanged += OnPrototypePropertyChanged;
        }

        private void OnPrototypePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged(_protoType, new PropertyChangedEventArgs(e.PropertyName));
        }

        #region ProtoType
        private IDockElement _protoType;

        public IDockElement ProtoType
        {
            get { return _protoType; }
        }
        #endregion

        #region Interface
        public int ID
        {
            get
            {
                return _protoType.ID;
            }
        }

        public string Title
        {
            get
            {
                return _protoType.Title;
            }
            set { _protoType.Title = value; }
        }

        public ImageSource ImageSource
        {
            get
            {
                return _protoType.ImageSource;
            }
        }

        public object Content
        {
            get
            {
                return _protoType.Content;
            }
        }

        public DockSide Side
        {
            get
            {
                return _protoType.Side;
            }
        }

        public DockManager DockManager
        {
            get
            {
                return _protoType.DockManager;
            }
        }

        public double DesiredWidth
        {
            get
            {
                return _protoType.DesiredWidth;
            }

            set
            {
                _protoType.DesiredWidth = value;
            }
        }

        public double DesiredHeight
        {
            get
            {
                return _protoType.DesiredHeight;
            }

            set
            {
                _protoType.DesiredHeight = value;
            }
        }

        public double FloatLeft
        {
            get
            {
                return _protoType.FloatLeft;
            }

            set
            {
                _protoType.FloatLeft = value;
            }
        }

        public double FloatTop
        {
            get
            {
                return _protoType.FloatTop;
            }

            set
            {
                _protoType.FloatTop = value;
            }
        }

        public bool IsDocument
        {
            get { return _protoType.IsDocument; }
        }

        public DockMode Mode
        {
            get
            {
                return _protoType.Mode;
            }
        }

        public bool IsVisible
        {
            get { return _protoType.IsVisible; }
        }

        public bool IsActive
        {
            get { return _protoType.IsActive; }
        }

        public bool CanSelect
        {
            get { return _protoType.CanSelect; }
        }

        public ILayoutGroup Container
        {
            get { return _protoType.Container; }
        }

        public bool IsDocked => _protoType.IsDocked;

        public bool IsFloat => _protoType.IsFloat;

        public bool IsAutoHide => _protoType.IsAutoHide;

        /// <summary>
        /// 是否可以转为浮动模式
        /// </summary>
        public bool CanFloat
        {
            get
            {
                return _protoType == null ? false : _protoType.CanFloat;
            }
        }

        /// <summary>
        /// 是否可以转为Dock模式
        /// </summary>
        public bool CanDock
        {
            get
            {
                return _protoType == null ? false : _protoType.CanDock;
            }
        }

        /// <summary>
        /// 是否可以转为Document模式
        /// </summary>
        public bool CanDockAsDocument
        {
            get
            {
                return _protoType == null ? false : _protoType.CanDockAsDocument;
            }
        }

        /// <summary>
        /// 是否可以切换自动隐藏状态
        /// </summary>
        public bool CanSwitchAutoHideStatus
        {
            get
            {
                return _protoType == null ? false : _protoType.CanSwitchAutoHideStatus;
            }
        }

        /// <summary>
        /// 是否可以隐藏
        /// </summary>
        public bool CanHide
        {
            get
            {
                return _protoType == null ? false : _protoType.CanHide;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion

        /// <summary>
        /// 通用显示的方法。
        /// 显示的模式（Dock，Float，AnchorSide）与当前Status有关
        /// </summary>
        public void Show(bool toActice = true)
        {
            if (IsVisible && IsActive) return;
            if (Mode == DockMode.Float)
            {
                if (!IsDocument)
                    ToFloat(toActice);
                else ToDockAsDocument(toActice);
            }
            else Container.ShowWithActive(_protoType, toActice);
        }

        /// <summary>
        /// 此方法会直接从用户界面隐藏该项（CanSelect设为False）
        /// </summary>
        public void Hide()
        {
            if (Content is IDockDocSource
                && !(Content as IDockDocSource).AllowClose())
                return;
            _protoType?.Hide();
        }

        /// <summary>
        /// 转为浮动窗口
        /// </summary>
        public void ToFloat(bool isActive = true)
        {
            _protoType?.ToFloat(isActive);
        }

        /// <summary>
        /// 转为Dock模式
        /// </summary>
        public void ToDock(bool isActive = true)
        {
            _protoType?.ToDock(isActive);
        }

        /// <summary>
        /// 转为Document模式
        /// </summary>
        public void ToDockAsDocument(bool isActive = true)
        {
            _protoType?.ToDockAsDocument(isActive);
        }

        /// <summary>
        /// 转为Document模式
        /// </summary>
        public void ToDockAsDocument(int index, bool isActive = true)
        {
            _protoType?.ToDockAsDocument(index, isActive);
        }

        /// <summary>
        /// 在Normal和DockBar模式间切换
        /// </summary>
        public void SwitchAutoHideStatus()
        {
            _protoType?.SwitchAutoHideStatus();
        }

        /// <summary>
        /// 将CanSelect设为False，并从界面移除此项
        /// 对于Normal or Float模式，效果与Hide方法相同
        /// </summary>
        public void Close()
        {
            Hide();
        }

        public void SetActive(bool _isActive = true)
        {
            if (_isActive)
                _protoType.Container.ShowWithActive(_protoType, _isActive);
            else if(DockManager.ActiveElement == _protoType)
                DockManager.ActiveElement = null;
        }

        private bool _isDisposed = false;
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            DockManager.RemoveDockControl(this);
            _protoType.PropertyChanged -= OnPrototypePropertyChanged;
            _protoType.Dispose();
            _protoType = null;
        }
    }
}