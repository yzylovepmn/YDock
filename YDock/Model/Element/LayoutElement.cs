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
    public class LayoutElement : DependencyObject, ILayoutElement, IComparable<LayoutElement>
    {
        public LayoutElement()
        {
        }


        #region Content
        private UIElement _content;
        public UIElement Content
        {
            get { return _content; }
            set
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
            DependencyProperty.Register("Title", typeof(string), typeof(LayoutElement),
                new FrameworkPropertyMetadata(string.Empty));

        public string Title
        {
            set { SetValue(TitleProperty, value); }
            get { return (string)GetValue(TitleProperty); }
        }
        #endregion

        #region ImageSource
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(LayoutElement));

        public ImageSource ImageSource
        {
            set { SetValue(ImageSourceProperty, value); }
            get { return (ImageSource)GetValue(ImageSourceProperty); }
        }
        #endregion

        #region DockSide
        public static readonly DependencyProperty SideProperty =
            DependencyProperty.Register("Side", typeof(DockSide), typeof(LayoutElement));

        public DockSide Side
        {
            set { SetValue(SideProperty, value); }
            get { return (DockSide)GetValue(SideProperty); }
        }
        #endregion

        #region IsVisible
        private bool isVisible = false;
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
        private bool _canSelect = true;
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
            set
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



        public YDock DockManager
        {
            get
            {
                return _container?.DockManager;
            }
        }

        public int CompareTo(LayoutElement other)
        {
            return Title.CompareTo(other.Title);
        }
    }
}