using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class LayoutContentControl : Control, INotifyPropertyChanged, IDisposable
    {
        static LayoutContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutContentControl), new FrameworkPropertyMetadata(typeof(LayoutContentControl)));
        }

        public LayoutContentControl()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(LayoutElement), typeof(LayoutContentControl));

        public LayoutElement Model
        {
            get { return (LayoutElement)GetValue(ModelProperty); }
            set
            {
                SetValue(ModelProperty, value);
            }
        }

        public void Dispose()
        {
            Model = null;
            DataContext = null;
        }
    }
}