using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class DocumentTabControl : TabControl, IView
    {
        static DocumentTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentTabControl), new FrameworkPropertyMetadata(typeof(DocumentTabControl)));
            FocusableProperty.OverrideMetadata(typeof(DocumentTabControl), new FrameworkPropertyMetadata(false));
        }

        internal DocumentTabControl(DocumentTab model)
        {
            Model = model;
            SetBinding(ItemsSourceProperty, new Binding("Model.Children") { Source = this });
        }

        private IModel _model;
        public IModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model != null) _model.View = null;
                if (_model != value)
                {
                    _model = value;
                    _model.View = this;
                }
            }
        }
    }
}