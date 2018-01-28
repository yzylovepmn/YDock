using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class DockBarGroupControl : ItemsControl, IDockView
    {
        static DockBarGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockBarGroupControl), new FrameworkPropertyMetadata(typeof(DockBarGroupControl)));
            FocusableProperty.OverrideMetadata(typeof(DockBarGroupControl), new FrameworkPropertyMetadata(false));
        }

        public DockBarGroupControl(ILayoutGroup model)
        {
            Model = model;

            SetBinding(ItemsSourceProperty, new Binding("Model.Children_CanSelect") { Source = this });

            var transform = new RotateTransform();
            switch ((Model as ILayout).Side)
            {
                case DockSide.Left:
                case DockSide.Right:
                    transform.Angle = 90;
                    break;
            }
            LayoutTransform = transform;
        }



        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ILayoutGroup _model;

        public IDockModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model != null) (_model as DockSideGroup).View = null;
                if (_model != value)
                {
                    _model = value as ILayoutGroup;
                    (_model as DockSideGroup).View = this;
                }
            }
        }

        public IEnumerable<IDockElement> Children
        {
            get
            {
                return Items.Cast<IDockElement>();
            }
        }

        public DockSide Side
        {
            get
            {
                return _model.Side;
            }
        }

        public IDockView DockViewParent
        {
            get
            {
                return _model.DockManager;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DockBarItemControl(this);
        }

        public void Dispose()
        {
            BindingOperations.ClearBinding(this, ItemsSourceProperty);
            Items.Clear();
            Model = null;
        }
    }
}