using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;

namespace YDock.Model
{
    public class DockElement : DependencyObject, IDockElement, IComparable<DockElement>
    {
        internal DockElement()
        {
        }


        #region Content
        private UIElement _content;
        public UIElement Content
        {
            get { return _content; }
            internal set
            {
                if (_content != value)
                {
                    _content = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Content"));
                }
            }
        }
        #endregion


        #region Title
        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register("Title", typeof(string), typeof(DockElement),
                new FrameworkPropertyMetadata(string.Empty));

        public string Title
        {
            internal set { SetValue(TitleProperty, value); }
            get { return (string)GetValue(TitleProperty); }
        }
        #endregion

        #region ImageSource
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(DockElement));

        public ImageSource ImageSource
        {
            internal set { SetValue(ImageSourceProperty, value); }
            get { return (ImageSource)GetValue(ImageSourceProperty); }
        }
        #endregion

        #region DockSide
        public static readonly DependencyProperty SideProperty =
            DependencyProperty.Register("Side", typeof(DockSide), typeof(DockElement));

        public DockSide Side
        {
            internal set { SetValue(SideProperty, value); }
            get { return (DockSide)GetValue(SideProperty); }
        }
        #endregion

        #region ID
        private int _id;
        public int ID
        {
            get { return _id; }
            internal set
            {
                if (_id != value)
                    _id = value;
            }
        }
        #endregion

        #region DockStatus
        private DockStatus _status;
        public DockStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }
        #endregion

        #region IsVisible
        private bool isVisible = false;
        /// <summary>
        /// Content是否可见
        /// </summary>
        public bool IsVisible
        {
            internal set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsVisible"));
                }
            }
            get { return isVisible; }
        }
        #endregion

        #region IsActive
        private bool _isActive = false;
        /// <summary>
        /// 是否为当前的活动窗口
        /// </summary>
        public bool IsActive
        {
            internal set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsActive"));
                }
            }
            get { return _isActive; }
        }
        #endregion

        #region CanSelect
        private bool _canSelect = false;
        /// <summary>
        /// 是否显示在用户界面供选择，默认为false
        /// </summary>
        public bool CanSelect
        {
            internal set
            {
                if (_canSelect != value)
                {
                    _canSelect = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CanSelect"));
                }
            }
            get { return _canSelect; }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ILayoutGroup _container;
        public ILayoutGroup Container
        {
            get
            {
                return _container;
            }
            internal set
            {
                if (_container != value)
                    _container = value;
            }
        }

        private double _desiredWidth;
        public double DesiredWidth
        {
            get
            {
                return _desiredWidth;
            }
            set
            {
                _desiredWidth = value;
            }
        }

        private double _desiredHeight;
        public double DesiredHeight
        {
            get
            {
                return _desiredHeight;
            }
            set
            {
                _desiredHeight = value;
            }
        }

        private double _floatLeft;
        public double FloatLeft
        {
            get { return _floatLeft; }
            set
            {
                if (_floatLeft != value)
                    _floatLeft = value;
            }
        }

        private double _floatTop;
        public double FloatTop
        {
            get { return _floatTop; }
            set
            {
                if (_floatTop != value)
                    _floatTop = value;
            }
        }


        public DockManager DockManager
        {
            get
            {
                return _container?.DockManager;
            }
        }

        public int CompareTo(DockElement other)
        {
            return Title.CompareTo(other.Title);
        }

        public void Dispose()
        {
            _content = null;
            _container = null;
        }
    }
}