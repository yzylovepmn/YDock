using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDock.Interface;

namespace YDock.View
{
    public class BaseDropPanel : BaseRenderPanel
    {
        internal BaseDropPanel(IDragTarget target, DragItem source)
        {
            _target = target;
            _source = source;
        }

        protected IDragTarget _target;
        /// <summary>
        /// 拖放目标
        /// </summary>
        internal IDragTarget Target
        {
            get { return _target; }
        }

        protected DragItem _source;
        /// <summary>
        /// 拖放源
        /// </summary>
        internal DragItem Source
        {
            get { return _source; }
        }

        public override void Dispose()
        {
            _target = null;
            _source = null;
            base.Dispose();
        }
    }

    public class RootDropPanel : BaseDropPanel
    {
        internal RootDropPanel(IDragTarget target, DragItem source) : base(target, source)
        {
            //绘制停靠的区域
            AddChild(new ActiveRectDropVisual(DragManager.NONE));
            //绘制左边的拖放区域
            AddChild(new UnitDropVisual(DragManager.LEFT));
            //绘制顶部的拖放区域
            AddChild(new UnitDropVisual(DragManager.TOP));
            //绘制右边的拖放区域
            AddChild(new UnitDropVisual(DragManager.RIGHT));
            //绘制底部的拖放区域
            AddChild(new UnitDropVisual(DragManager.BOTTOM));
        }
    }

    public class DropPanel : BaseDropPanel
    {
        internal DropPanel(IDragTarget target, DragItem source) : base(target, source)
        {
            //绘制停靠的区域
            AddChild(new ActiveRectDropVisual(DragManager.NONE));
            //绘制拖放区域的玻璃外观
            AddChild(new GlassDropVisual(DragManager.NONE));
            //绘制中心的拖放区域
            AddChild(new UnitDropVisual(DragManager.CENTER));
            //绘制左边的拖放区域
            AddChild(new UnitDropVisual(DragManager.LEFT));
            //绘制顶部的拖放区域
            AddChild(new UnitDropVisual(DragManager.TOP));
            //绘制右边的拖放区域
            AddChild(new UnitDropVisual(DragManager.RIGHT));
            //绘制底部的拖放区域
            AddChild(new UnitDropVisual(DragManager.BOTTOM));
            //绘制左分割的拖放区域
            AddChild(new UnitDropVisual(DragManager.LEFT | DragManager.SPLIT));
            //绘制上分割的拖放区域
            AddChild(new UnitDropVisual(DragManager.TOP | DragManager.SPLIT));
            //绘制右分割的拖放区域
            AddChild(new UnitDropVisual(DragManager.RIGHT | DragManager.SPLIT));
            //绘制下分割的拖放区域
            AddChild(new UnitDropVisual(DragManager.BOTTOM | DragManager.SPLIT));
        }
    }
}