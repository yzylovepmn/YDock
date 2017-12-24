using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class AnchorDocumentControl : Control
    {
        static AnchorDocumentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorDocumentControl), new FrameworkPropertyMetadata(typeof(AnchorDocumentControl)));
        }

        public AnchorDocumentControl(LayoutElement model)
        {
            _model = model;
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