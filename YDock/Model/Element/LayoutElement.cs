using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using YDock.Interface;

namespace YDock.Model
{
    public class LayoutElement : DependencyObject, ILayoutElement
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
            DependencyProperty.Register("Title", typeof(string), typeof(LayoutElement));

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
        private bool isActive = false;
        public bool IsActive
        {
            internal set
            {
                if (isActive != value)
                {
                    isActive = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsActive"));
                }
            }
            get { return isActive; }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ILayoutContainer _container;
        public ILayoutContainer Container
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

        private double _actualWidth;
        public double ActualWidth
        {
            get
            {
                return _actualWidth;
            }
            internal set
            {
                _actualWidth = value;
            }
        }

        private double _actualHeight;
        public double ActualHeight
        {
            get
            {
                return _actualHeight;
            }
            internal set
            {
                _actualHeight = value;
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

        public bool IsDock
        {
            get
            {
                return _container != null && _container is YDockSide;
            }
        }

        public bool IsDocument
        {
            get
            {
                return _container != null && _container is DocumentTab;
            }
        }
    }
}