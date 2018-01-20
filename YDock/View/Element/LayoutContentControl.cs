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


        protected LayoutElement _model;
        public LayoutElement Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Model"));
                }
            }
        }

        public void Dispose()
        {
            _model = null;
            DataContext = null;
        }
    }
}