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
    public class DockSideItemControl : ContentControl, IDisposable
    {
        static DockSideItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockSideItemControl), new FrameworkPropertyMetadata(typeof(DockSideItemControl)));
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var ele = Content as LayoutElement;
            if (ele == _container.DockManager.AutoHideElement)
            {
                _container.DockManager.ActiveElement = null;
                _container.DockManager.AutoHideElement = null;
            }
            else
            {
                _container.DockManager.ActiveElement = ele;
                _container.DockManager.AutoHideElement = ele;
            }
            base.OnMouseLeftButtonDown(e);
        }

        public void Dispose()
        {
            _container = null;
        }
    }
}