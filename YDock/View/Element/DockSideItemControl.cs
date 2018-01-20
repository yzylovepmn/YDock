using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class DockSideItemControl : ContentControl
    {
        static DockSideItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockSideItemControl), new FrameworkPropertyMetadata(typeof(DockSideItemControl)));
            FocusableProperty.OverrideMetadata(typeof(DockSideItemControl), new FrameworkPropertyMetadata(false));
        }

        public DockSideItemControl(ILayoutGroup container)
        {
            _container = container;
        }

        private ILayoutGroup _container;
        public ILayoutGroup Container
        {
            get
            {
                return _container;
            }

            set
            {
                if (_container != value)
                    _container = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _container.DockManager.AutoHideElement = Content as ILayoutElement;
            base.OnMouseLeftButtonDown(e);
        }
    }
}