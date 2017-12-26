using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
        }

        private LayoutElement _model;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
    }
}