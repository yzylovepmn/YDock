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
    }
}