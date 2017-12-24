using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class YDockSideControl : Control, IView
    {
        static YDockSideControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YDockSideControl), new FrameworkPropertyMetadata(typeof(YDockSideControl)));
            FocusableProperty.OverrideMetadata(typeof(YDockSideControl), new FrameworkPropertyMetadata(false));
        }

        public YDockSideControl(IAnchorModel model)
        {
            Model = model;

            _CreateViewChildren();

            ((YDockSide)Model).Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }

        private IAnchorModel _model;
        public IAnchorModel Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                    _model = value;
            }
        }

        IModel IView.Model
        {
            get
            {
                return _model;
            }
        }

        ObservableCollection<DocumentHeaderControl> _childViews = new ObservableCollection<DocumentHeaderControl>();

        public ObservableCollection<DocumentHeaderControl> Children
        {
            get { return _childViews; }
        }

        private void _CreateViewChildren()
        {
            if (Model == null) return;
            YDockSide dockSide = Model as YDockSide;
            foreach (var child in dockSide.Children)
                Children.Add(new DocumentHeaderControl(child));
        }
    }
}