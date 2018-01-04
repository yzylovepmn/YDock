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
                        Height = 0;
                        Width = 0;
                        MinHeight = 0;
                        MinWidth = 0;
                        PropertyChanged(this, new PropertyChangedEventArgs("Model"));
                    }
                    _model = value;
                    if (_model != null)
                    {
                        DockSide side = (_model.Container as YDockSide).Side;
                        if (side == DockSide.Left || side == DockSide.Right)
                            MinWidth = 30;
                        else MinHeight = 30;
                        Width = double.NaN;
                        Height = double.NaN;
                        (_model as LayoutElement).IsVisible = true;
                        PropertyChanged(this, new PropertyChangedEventArgs("Model"));
                    }
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (_model != null)
            {
                if (_model.ActualWidth <= 0.1 && _model.ActualHeight <= 0.1)
                {
                    (_model as LayoutElement).ActualWidth = ActualWidth;
                    (_model as LayoutElement).ActualHeight = ActualHeight;
                }
            }
        }
    }
}