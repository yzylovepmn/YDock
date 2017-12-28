using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class YDockSideControl : ItemsControl, IView, ILayoutContainer
    {
        static YDockSideControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YDockSideControl), new FrameworkPropertyMetadata(typeof(YDockSideControl)));
            FocusableProperty.OverrideMetadata(typeof(YDockSideControl), new FrameworkPropertyMetadata(false));
        }

        public YDockSideControl(IAnchorModel model)
        {
            Model = model;

            SetBinding(ItemsSourceProperty, new Binding("Model.Children") { Source = this });

            var transform = new RotateTransform();
            switch (Model.Side)
            {
                case Enum.DockSide.Left:
                    transform.Angle = 270;
                    break;
                case Enum.DockSide.Right:
                    transform.Angle = 90;
                    break;
            }
            LayoutTransform = transform;
        }


        private IAnchorModel _model;
        public IAnchorModel Model
        {
            get { return _model; }
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

        IModel IView.Model
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
                    _model = (IAnchorModel)value;
                    _model.View = this;
                }
            }
        }

        public IEnumerable<ILayoutElement> Children
        {
            get
            {
                return Items.Cast<ILayoutElement>();
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DockSideItemControl(this);
        }
    }
}