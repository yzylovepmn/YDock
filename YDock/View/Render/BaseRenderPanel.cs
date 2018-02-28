using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using YDock.Interface;

namespace YDock.View
{
    public class BaseRenderPanel : FrameworkElement, IDisposable
    {
        public BaseRenderPanel()
        {
            _children = new List<BaseVisual>();
            DataContextChanged += OnDataContextChanged;
        }

        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private IList<BaseVisual> _children;
        public IList<BaseVisual> Children
        {
            get { return _children; }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                if (_children == null) return 0;
                return _children.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _children[index];
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            foreach (var child in _children)
                child.Update(sizeInfo.NewSize);
        }

        public void AddChild(BaseVisual child)
        {
            _children.Add(child);
            AddLogicalChild(child);
            AddVisualChild(child);
        }

        public void RemoveChild(BaseVisual child)
        {
            _children.Remove(child);
            RemoveLogicalChild(child);
            RemoveVisualChild(child);
        }

        public virtual void UpdateChildren()
        {
            foreach (var child in _children)
                child.Update(new Size(ActualWidth, ActualHeight));
        }

        public virtual void Dispose()
        {
            DataContext = null;
            _children.Clear();
            _children = null;
        }
    }

    public class TexturePanel : BaseRenderPanel
    {
        public TexturePanel()
        {
            AddChild(new TextureHeaderVisual());
        }

        protected override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            base.OnDataContextChanged(sender, e);
            if (e.OldValue != null && e.OldValue is IDockElement)
                (e.OldValue as IDockElement).PropertyChanged -= OnModelPropertyChanged;
            if (e.NewValue != null && e.NewValue is IDockElement)
                (e.NewValue as IDockElement).PropertyChanged += OnModelPropertyChanged;
        }

        private void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsActive")
                UpdateChildren();
        }
    }
}