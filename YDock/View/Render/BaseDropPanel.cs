using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;

namespace YDock.View
{
    public class BaseDropPanel : BaseRenderPanel
    {
        private static BaseDropVisual _activeVisual;
        internal static BaseDropVisual ActiveVisual
        {
            get { return _activeVisual; }
            set
            {
                if (_activeVisual != value)
                {
                    if (_activeVisual != null)
                    {
                        _activeVisual.Flag &= ~DragManager.ACTIVE;
                        _activeVisual.Update();
                    }
                    _activeVisual = value;
                    if (_activeVisual != null)
                    {
                        _activeVisual.Flag |= DragManager.ACTIVE;
                        _activeVisual.Update();
                    }
                }
            }
        }

        private static ActiveRectDropVisual _currentRect;
        internal static ActiveRectDropVisual CurrentRect
        {
            get { return _currentRect; }
            set
            {
                if (_currentRect != value)
                {
                    if (_currentRect != null)
                    {
                        _currentRect.DropPanel._target.DropMode = DropMode.None;
                        _currentRect.Flag = DragManager.NONE;
                        _currentRect.Update();
                    }
                    _currentRect = value;
                    if (_currentRect != null)
                        _currentRect.Update();
                }
                else _currentRect?.Update();
            }
        }

        internal BaseDropPanel(IDragTarget target, DragItem source)
        {
            _target = target;
            _source = source;
            //绘制停靠的区域
            _activeRect = new ActiveRectDropVisual(DragManager.NONE);
            AddChild(_activeRect);
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

        private ActiveRectDropVisual _activeRect;
        public ActiveRectDropVisual ActiveRect { get { return _activeRect; } }

        private Rect _innerRect;
        public Rect InnerRect
        {
            get { return _innerRect; }
            set { _innerRect = value; }
        }

        private Rect _outerRect;
        public Rect OuterRect
        {
            get { return _outerRect; }
            set { _outerRect = value; }
        }

        public void Update(Point mouseP)
        {
            var p = this.PointToScreenDPIWithoutFlowDirection(new Point());
            var result = VisualTreeHelper.HitTest(this, new Point(mouseP.X - p.X, mouseP.Y - p.Y));
            if (result?.VisualHit != null && result?.VisualHit is UnitDropVisual)
            {
                UnitDropVisual visual = result?.VisualHit as UnitDropVisual;
                ActiveVisual = visual;
                var mode = _GetMode(visual.Flag);
                if (mode == _target.DropMode)
                    return;
                _target.DropMode = mode;

                _activeRect.Flag = visual.Flag;
                _activeRect.Rect = Rect.Empty;
            }
            else
            {
                ActiveVisual = null;
                if (_target is BaseGroupControl)
                {
                    (_target as BaseGroupControl).HitTest(mouseP, _activeRect);
                    if (!(_target as BaseGroupControl).canUpdate)
                        return;
                    _target.DropMode = _GetMode(_activeRect.Flag);
                }
                else
                {
                    _target.DropMode = DropMode.None;
                    _activeRect.Flag = DragManager.NONE;
                }
            }

            if (this is RootDropPanel)
            {
                _target.DockManager.DragManager.IsDragOverRoot = ActiveVisual != null;
                if (_target.DropMode != DropMode.None && _currentRect != null)
                {
                    _currentRect.Flag = DragManager.NONE;
                    _currentRect.DropPanel._target.DropMode = DropMode.None;
                    _currentRect.Update();
                }
                _activeRect.Update();
            }
            if (!(this is RootDropPanel))
                CurrentRect = _activeRect;
        }

        DropMode _GetMode(int flag)
        {
            if ((flag & DragManager.HEAD) != 0)
                return DropMode.Header;
            if ((flag & DragManager.LEFT) != 0)
            {
                if ((flag & DragManager.SPLIT) != 0)
                    return DropMode.Left_WithSplit;
                else return DropMode.Left;
            }
            if ((flag & DragManager.RIGHT) != 0)
            {
                if ((flag & DragManager.SPLIT) != 0)
                    return DropMode.Right_WithSplit;
                else return DropMode.Right;
            }
            if ((flag & DragManager.TOP) != 0)
            {
                if ((flag & DragManager.SPLIT) != 0)
                    return DropMode.Top_WithSplit;
                else return DropMode.Top;
            }
            if ((flag & DragManager.BOTTOM) != 0)
            {
                if ((flag & DragManager.SPLIT) != 0)
                    return DropMode.Bottom_WithSplit;
                else return DropMode.Bottom;
            }
            if ((flag & DragManager.CENTER) != 0)
                return DropMode.Center;
            return DropMode.None;
        }

        public override void Dispose()
        {
            _target = null;
            _source = null;
            _activeRect = null;
            base.Dispose();
        }
    }

    public class RootDropPanel : BaseDropPanel
    {
        internal RootDropPanel(IDragTarget target, DragItem source) : base(target, source)
        {
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