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
    public class LayoutDocumentGroupControl : TabControl, ILayoutGroupControl
    {
        static LayoutDocumentGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentGroupControl), new FrameworkPropertyMetadata(typeof(LayoutDocumentGroupControl)));
            FocusableProperty.OverrideMetadata(typeof(LayoutDocumentGroupControl), new FrameworkPropertyMetadata(false));
        }

        internal LayoutDocumentGroupControl(ILayoutGroup model)
        {
            Model = model;
            SetBinding(ItemsSourceProperty, new Binding("Model.Children_CanSelect") { Source = this });
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
                if (_model != value)
                {
                    if (_model != null)
                        _model.View = null;
                    _model = value;
                    if (_model != null)
                        _model.View = this;
                }
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.RemovedItems != null)
                foreach (LayoutElement item in e.RemovedItems)
                    item.IsVisible = false;

            if (e.AddedItems != null)
                foreach (LayoutElement item in e.AddedItems)
                    item.IsVisible = true;
        }

        private double _desiredWidth;
        public double DesiredWidth
        {
            get
            {
                return _desiredWidth;
            }
            set
            {
                if (_desiredWidth != value)
                    _desiredWidth = value;
            }
        }

        private double _desiredHeight;
        public double DesiredHeight
        {
            get
            {
                return _desiredHeight;
            }
            set
            {
                if (_desiredHeight != value)
                    _desiredHeight = value;
            }
        }
    }
}