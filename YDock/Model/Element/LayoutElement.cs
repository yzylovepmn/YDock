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
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(LayoutElement));


        public bool IsVisible
        {
            set { SetValue(IsVisibleProperty, value); }
            get { return (bool)GetValue(ImageSourceProperty); }
        }

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
        #endregion
    }
}