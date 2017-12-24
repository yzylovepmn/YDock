using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YDock.Model;

namespace YDock.View
{
    public class DocumentHeaderControl : Control
    {
        static DocumentHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentHeaderControl), new FrameworkPropertyMetadata(typeof(DocumentHeaderControl)));
            FocusableProperty.OverrideMetadata(typeof(DocumentHeaderControl), new FrameworkPropertyMetadata(false));
        }

        public DocumentHeaderControl() { }

        public DocumentHeaderControl(LayoutElement model)
        {
            Model = model;
        }

        private LayoutElement _model;
        public LayoutElement Model
        {
            get { return _model; }
            internal set
            {
                if (_model != value)
                    _model = value;
            }
        }
    }
}