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
    public class DockSideItemControl : ContentControl, IDockView
    {
        static DockSideItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockSideItemControl), new FrameworkPropertyMetadata(typeof(DockSideItemControl)));
        }

        public DockSideItemControl(IDockView dockViewParent)
        {
            _dockViewParent = dockViewParent;
        }

        public ILayoutGroup Container
        {
            get
            {
                return _dockViewParent?.Model as ILayoutGroup;
            }
        }

        public IDockModel Model
        {
            get
            {
                return null;
            }
        }

        private IDockView _dockViewParent;
        public IDockView DockViewParent
        {
            get
            {
                return _dockViewParent;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var ele = Content as DockElement;
            if (ele == Container.DockManager.AutoHideElement)
            {
                Container.DockManager.ActiveElement = null;
                Container.DockManager.AutoHideElement = null;
            }
            else
            {
                Container.DockManager.ActiveElement = ele;
                Container.DockManager.AutoHideElement = ele;
            }
            base.OnMouseLeftButtonDown(e);
        }

        public void Dispose()
        {
            _dockViewParent = null;
        }
    }
}