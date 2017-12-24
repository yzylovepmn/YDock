using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace YDock.Model
{
    public class LayoutElement : DependencyObject
    {
        public LayoutElement()
        {
        }


        #region Content
        private UIElement _content;
        public UIElement Content
        {
            get { return _content; }
            set { _content = value; }
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
    }
}