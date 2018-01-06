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
    public class AnchorDocumentControl : Control, INotifyPropertyChanged
    {
        static AnchorDocumentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorDocumentControl), new FrameworkPropertyMetadata(typeof(AnchorDocumentControl)));
        }

        public AnchorDocumentControl()
        {
            Height = 0;
            Width = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        private ILayoutElement _model;
        public ILayoutElement Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    if (_model != null)
                    {
                        (_model as LayoutElement).IsVisible = false;
                        _model = null;
                        Width = double.NaN;
                        Height = double.NaN;
                    }
                    _model = value;
                    if (_model != null)
                    {
                        Width = double.NaN;
                        Height = double.NaN;
                        (_model as LayoutElement).IsVisible = true;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("Model"));
                }
            }
        }
    }
}