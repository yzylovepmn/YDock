using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace YDock.View
{
    public class BaseRenderPanel : FrameworkElement
    {
        public BaseRenderPanel()
        {
            _children = new List<BaseVisual>();
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
    }

    public class TexturePanel : BaseRenderPanel
    {
        public TexturePanel()
        {
            AddChild(new TextureHeaderVisual());
        }
    }
}