using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using YDock.Enum;

namespace YDock.View
{
    public abstract class BaseDropVisual : BaseVisual
    {
        internal BaseDropVisual(int flag)
        {
            Flag = flag;
        }

        private int _flag;
        internal virtual int Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }

        public BaseDropPanel DropPanel
        {
            get { return VisualParent as BaseDropPanel; }
        }

        public void Update()
        {
            Update(new Size(DropPanel.ActualWidth, DropPanel.ActualHeight));
        }
    }

    public class GlassDropVisual : BaseDropVisual
    {
        internal GlassDropVisual(int flag) : base(flag)
        {

        }

        public override void Update(Size size)
        {
            using (var ctx = RenderOpen())
            {
                double hoffeset = 0, voffeset = 0, sideLength = 0;
                if (DropPanel.Target.Mode == DragMode.Document
                    && DropPanel.Source.DragMode == DragMode.Anchor)
                {
                    hoffeset = size.Width / 2 - Constants.DropUnitLength * 5 / 2 + DropPanel.Hoffset;
                    voffeset = size.Height / 2 - Constants.DropUnitLength / 2 + DropPanel.Voffset;
                    sideLength = Constants.DropUnitLength * 2 - Constants.DropCornerLength;
                }
                else if(DropPanel.Target.Mode == DragMode.None || DropPanel.Source.DragMode == DragMode.None || (DropPanel.Target.Mode == DragMode.Anchor
                    && DropPanel.Source.DragMode == DragMode.Document))
                    return;
                else
                {
                    hoffeset = size.Width / 2 - Constants.DropUnitLength * 3 / 2 + DropPanel.Hoffset;
                    voffeset = size.Height / 2 - Constants.DropUnitLength / 2 + DropPanel.Voffset;
                    sideLength = Constants.DropUnitLength - Constants.DropCornerLength;
                }

                ctx.PushOpacity(Constants.DragOpacity);
                List<Point> points = new List<Point>();
                double currentX = hoffeset, currentY = voffeset;

                points.Add(new Point(currentX += sideLength, currentY));
                points.Add(new Point(currentX += Constants.DropCornerLength, currentY -= Constants.DropCornerLength));
                points.Add(new Point(currentX, currentY -= sideLength));
                points.Add(new Point(currentX += Constants.DropUnitLength, currentY));
                points.Add(new Point(currentX, currentY += sideLength));
                points.Add(new Point(currentX += Constants.DropCornerLength, currentY += Constants.DropCornerLength));
                points.Add(new Point(currentX += sideLength, currentY));
                points.Add(new Point(currentX, currentY += Constants.DropUnitLength));
                points.Add(new Point(currentX -= sideLength, currentY));
                points.Add(new Point(currentX -= Constants.DropCornerLength, currentY += Constants.DropCornerLength));
                points.Add(new Point(currentX, currentY += sideLength));
                points.Add(new Point(currentX -= Constants.DropUnitLength, currentY));
                points.Add(new Point(currentX, currentY -= sideLength));
                points.Add(new Point(currentX -= Constants.DropCornerLength, currentY -= Constants.DropCornerLength));
                points.Add(new Point(currentX -= sideLength, currentY));
                points.Add(new Point(currentX, currentY -= Constants.DropUnitLength));

                StreamGeometry stream = new StreamGeometry();
                using (var sctx = stream.Open())
                {
                    sctx.BeginFigure(new Point(currentX, currentY), true, true);
                    sctx.PolyLineTo(points, true, true);
                    sctx.Close();
                }
                ctx.DrawGeometry(Brushes.White, ResourceManager.BorderPen, stream);
            }
        }
    }

    public class ActiveRectDropVisual : BaseDropVisual
    {
        internal ActiveRectDropVisual(int flag) : base(flag)
        {

        }

        public override void Update(Size size)
        {
            
        }

        private double _length;
        public double Length
        {
            get { return _length; }
            set { _length = value; }
        }

        private double? _headerLength;
        public double? HeaderLength
        {
            get { return _headerLength; }
            set { _headerLength = value; }
        }

        private double? _offset;
        new public double? Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
    }

    public class UnitDropVisual: BaseDropVisual
    {
        internal UnitDropVisual(int flag) : base(flag)
        {

        }

        public override void Update(Size size)
        {
            using (var ctx = RenderOpen())
            {
                double hoffset = 0, voffset = 0;
                if (DropPanel is RootDropPanel)
                {
                    if ((Flag & DragManager.LEFT) != 0)
                        _DrawLeft(ctx, Constants.DropGlassLength, (size.Height - Constants.DropUnitLength) / 2);
                    else if ((Flag & DragManager.TOP) != 0)
                        _DrawTop(ctx, (size.Width - Constants.DropUnitLength) / 2, Constants.DropGlassLength);
                    else if ((Flag & DragManager.RIGHT) != 0)
                        _DrawRight(ctx, size.Width - Constants.DropGlassLength, (size.Height - Constants.DropUnitLength) / 2);
                    else if ((Flag & DragManager.BOTTOM) != 0)
                        _DrawBottom(ctx, (size.Width - Constants.DropUnitLength) / 2, size.Height - Constants.DropGlassLength);
                }
                else
                {
                    if ((Flag & DragManager.CENTER) != 0)
                    {
                        bool flag = false;
                        if (DropPanel.Target.Mode == DragMode.Document)
                            flag = true;
                        if (DropPanel.Source.DragMode != DragMode.Document
                            && DropPanel.Target.Mode == DragMode.Anchor)
                            flag = true;

                        if (flag)
                            _DrawCenter(ctx, (size.Width - Constants.DropUnitLength) / 2, (size.Height - Constants.DropUnitLength) / 2);
                    }

                    if ((Flag & DragManager.LEFT) != 0)
                    {
                        if ((Flag & DragManager.SPLIT) != 0)
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                hoffset = (size.Width - Constants.DropUnitLength) / 2 - Constants.DropUnitLength;
                                voffset = (size.Height - Constants.DropUnitLength) / 2;
                                _DrawCenter(ctx, hoffset, voffset, true, true);
                            }
                        }
                        else
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                hoffset = (size.Width - Constants.DropUnitLength) / 2 - Constants.DropUnitLength * 2;
                                voffset = (size.Height - Constants.DropUnitLength) / 2;
                            }
                            else
                            {
                                hoffset = (size.Width - Constants.DropUnitLength) / 2 - Constants.DropUnitLength;
                                voffset = (size.Height - Constants.DropUnitLength) / 2;
                            }
                            if (DropPanel.Source.DragMode == DragMode.Anchor)
                                _DrawLeft(ctx, hoffset, voffset, false);
                        }
                    }

                    if ((Flag & DragManager.RIGHT) != 0)
                    {
                        if ((Flag & DragManager.SPLIT) != 0)
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                hoffset = (size.Width + Constants.DropUnitLength) / 2;
                                voffset = (size.Height - Constants.DropUnitLength) / 2;
                                _DrawCenter(ctx, hoffset, voffset, true, true);
                            }
                        }
                        else
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                hoffset = size.Width / 2 + Constants.DropUnitLength * 2.5;
                                voffset = (size.Height - Constants.DropUnitLength) / 2;
                            }
                            else
                            {
                                hoffset = size.Width / 2 + Constants.DropUnitLength * 1.5;
                                voffset = (size.Height - Constants.DropUnitLength) / 2;
                            }
                            if (DropPanel.Source.DragMode == DragMode.Anchor)
                                _DrawRight(ctx, hoffset, voffset, false);
                        }
                    }

                    if ((Flag & DragManager.TOP) != 0)
                    {
                        if ((Flag & DragManager.SPLIT) != 0)
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                hoffset = (size.Width - Constants.DropUnitLength) / 2;
                                voffset = size.Height / 2 - Constants.DropUnitLength * 1.5;
                                _DrawCenter(ctx, hoffset, voffset, true);
                            }
                        }
                        else
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                hoffset = (size.Width - Constants.DropUnitLength) / 2;
                                voffset = size.Height / 2 - Constants.DropUnitLength * 2.5;
                            }
                            else
                            {
                                hoffset = (size.Width - Constants.DropUnitLength) / 2;
                                voffset = size.Height / 2 - Constants.DropUnitLength * 1.5;
                            }
                            if (DropPanel.Source.DragMode == DragMode.Anchor)
                                _DrawTop(ctx, hoffset, voffset, false);
                        }
                    }

                    if ((Flag & DragManager.BOTTOM) != 0)
                    {
                        if ((Flag & DragManager.SPLIT) != 0)
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                hoffset = (size.Width - Constants.DropUnitLength) / 2;
                                voffset = (size.Height + Constants.DropUnitLength) / 2;
                                _DrawCenter(ctx, hoffset, voffset, true);
                            }
                        }
                        else
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                hoffset = (size.Width - Constants.DropUnitLength) / 2;
                                voffset = size.Height / 2 + Constants.DropUnitLength * 2.5;
                            }
                            else
                            {
                                hoffset = (size.Width - Constants.DropUnitLength) / 2;
                                voffset = size.Height / 2 + Constants.DropUnitLength * 1.5;
                            }
                            if (DropPanel.Source.DragMode == DragMode.Anchor)
                                _DrawBottom(ctx, hoffset, voffset, false);
                        }
                    }
                }
            }
        }

        private void _DrawLeft(DrawingContext ctx, double hoffset, double voffset, bool hasGlassBorder = true)
        {
            hoffset += DropPanel.Hoffset;
            voffset += +DropPanel.Voffset;
            if (hasGlassBorder)
            {
                //绘制玻璃外观
                ctx.PushOpacity(Constants.DragOpacity);
                ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffset, voffset, Constants.DropUnitLength, Constants.DropUnitLength));
                ctx.Pop();
            }
            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 2);
            ctx.DrawRectangle(Brushes.White, null, new Rect(hoffset += Constants.DropGlassLength, voffset += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2));
            hoffset += Constants.DropGlassLength;
            voffset += Constants.DropGlassLength;
            ctx.DrawLine(ResourceManager.DropRectPen_Heavy, new Point(hoffset - 0.5, voffset), new Point(hoffset + 12.5, voffset));

            //绘制小窗口
            StreamGeometry stream = new StreamGeometry();
            using (var sctx = stream.Open())
            {
                double currentX = hoffset, currentY = voffset;
                sctx.BeginFigure(new Point(currentX, currentY), true, false);
                sctx.LineTo(new Point(currentX, currentY += Constants.DropUnitLength - Constants.DropGlassLength * 4), true, true);
                sctx.LineTo(new Point(currentX += 12, currentY), true, true);
                sctx.LineTo(new Point(currentX, currentY -= Constants.DropUnitLength - Constants.DropGlassLength * 4), true, true);
                sctx.Close();
            }
            ctx.DrawGeometry(null, ResourceManager.DropRectPen, stream);

            //绘制方向箭头
            stream = new StreamGeometry();
            using (var sctx = stream.Open())
            {
                double currentX = hoffset + 20, currentY = voffset + (Constants.DropUnitLength - Constants.DropGlassLength * 4) / 2;
                sctx.BeginFigure(new Point(currentX, currentY), true, true);
                sctx.LineTo(new Point(currentX += 5, currentY -= 5), true, true);
                sctx.LineTo(new Point(currentX, currentY += 10), true, true);
                sctx.Close();
            }
            ctx.DrawGeometry(Brushes.Black, null, stream);
        }

        private void _DrawTop(DrawingContext ctx, double hoffset, double voffset, bool hasGlassBorder = true)
        {
            hoffset += DropPanel.Hoffset;
            voffset += +DropPanel.Voffset;
            if (hasGlassBorder)
            {
                //绘制玻璃外观
                ctx.PushOpacity(Constants.DragOpacity);
                ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffset, voffset, Constants.DropUnitLength, Constants.DropUnitLength));
                ctx.Pop();
            }
            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 2);

            ctx.DrawRectangle(Brushes.White, null, new Rect(hoffset += Constants.DropGlassLength, voffset += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2));
            hoffset += Constants.DropGlassLength;
            voffset += Constants.DropGlassLength;

            ctx.DrawLine(ResourceManager.DropRectPen_Heavy, new Point(hoffset - 0.5, voffset), new Point(hoffset + Constants.DropUnitLength - Constants.DropGlassLength * 4 + 0.5, voffset));

            //绘制小窗口
            StreamGeometry stream = new StreamGeometry();
            using (var sctx = stream.Open())
            {
                double currentX = hoffset, currentY = voffset;
                sctx.BeginFigure(new Point(currentX, currentY), true, false);
                sctx.LineTo(new Point(currentX, currentY += 12), true, true);
                sctx.LineTo(new Point(currentX += Constants.DropUnitLength - Constants.DropGlassLength * 4, currentY), true, true);
                sctx.LineTo(new Point(currentX, currentY -= 12), true, true);
                sctx.Close();
            }
            ctx.DrawGeometry(null, ResourceManager.DropRectPen, stream);

            //绘制方向箭头
            stream = new StreamGeometry();
            using (var sctx = stream.Open())
            {
                double currentX = hoffset + (Constants.DropUnitLength - Constants.DropGlassLength * 4) / 2, currentY = voffset + 20;
                sctx.BeginFigure(new Point(currentX, currentY), true, true);
                sctx.LineTo(new Point(currentX += 5, currentY += 5), true, true);
                sctx.LineTo(new Point(currentX -= 10, currentY), true, true);
                sctx.Close();
            }
            ctx.DrawGeometry(Brushes.Black, null, stream);
        }

        private void _DrawRight(DrawingContext ctx, double hoffset, double voffset, bool hasGlassBorder = true)
        {
            hoffset += DropPanel.Hoffset;
            voffset += +DropPanel.Voffset;
            if (hasGlassBorder)
            {
                //绘制玻璃外观
                ctx.PushOpacity(Constants.DragOpacity);
                ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffset - Constants.DropUnitLength, voffset, Constants.DropUnitLength, Constants.DropUnitLength));
                ctx.Pop();
            }

            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 2);
            ctx.DrawRectangle(Brushes.White, null, new Rect((hoffset -= Constants.DropGlassLength) - (Constants.DropUnitLength - Constants.DropGlassLength * 2), voffset += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2));
            hoffset -= Constants.DropGlassLength;
            voffset += Constants.DropGlassLength;
            ctx.DrawLine(ResourceManager.DropRectPen_Heavy, new Point(hoffset + 0.5, voffset), new Point(hoffset - 12.5, voffset));

            //绘制小窗口
            StreamGeometry stream = new StreamGeometry();
            using (var sctx = stream.Open())
            {
                double currentX = hoffset, currentY = voffset;
                sctx.BeginFigure(new Point(currentX, currentY), true, false);
                sctx.LineTo(new Point(currentX, currentY += Constants.DropUnitLength - Constants.DropGlassLength * 4), true, true);
                sctx.LineTo(new Point(currentX -= 12, currentY), true, true);
                sctx.LineTo(new Point(currentX, currentY -= Constants.DropUnitLength - Constants.DropGlassLength * 4), true, true);
                sctx.Close();
            }
            ctx.DrawGeometry(null, ResourceManager.DropRectPen, stream);

            //绘制方向箭头
            stream = new StreamGeometry();
            using (var sctx = stream.Open())
            {
                double currentX = hoffset - 20, currentY = voffset + (Constants.DropUnitLength - Constants.DropGlassLength * 4) / 2;
                sctx.BeginFigure(new Point(currentX, currentY), true, true);
                sctx.LineTo(new Point(currentX -= 5, currentY -= 5), true, true);
                sctx.LineTo(new Point(currentX, currentY += 10), true, true);
                sctx.Close();
            }
            ctx.DrawGeometry(Brushes.Black, null, stream);
        }

        private void _DrawBottom(DrawingContext ctx, double hoffset, double voffset, bool hasGlassBorder = true)
        {
            hoffset += DropPanel.Hoffset;
            voffset += +DropPanel.Voffset;
            if (hasGlassBorder)
            {
                //绘制玻璃外观
                ctx.PushOpacity(Constants.DragOpacity);
                ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffset, voffset - Constants.DropUnitLength, Constants.DropUnitLength, Constants.DropUnitLength));
                ctx.Pop();
            }

            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 2);

            ctx.DrawRectangle(Brushes.White, null, new Rect(hoffset += Constants.DropGlassLength, (voffset -= Constants.DropGlassLength) - (Constants.DropUnitLength - Constants.DropGlassLength * 2), Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2));
            hoffset += Constants.DropGlassLength;
            voffset -= Constants.DropGlassLength;

            ctx.DrawLine(ResourceManager.DropRectPen_Heavy, new Point(hoffset - 0.5, voffset - 12), new Point(hoffset + Constants.DropUnitLength - Constants.DropGlassLength * 4 + 0.5, voffset - 12));

            //绘制小窗口
            StreamGeometry stream = new StreamGeometry();
            using (var sctx = stream.Open())
            {
                double currentX = hoffset, currentY = voffset - 12;
                sctx.BeginFigure(new Point(currentX, currentY), true, false);
                sctx.LineTo(new Point(currentX, currentY += 12), true, true);
                sctx.LineTo(new Point(currentX += Constants.DropUnitLength - Constants.DropGlassLength * 4, currentY), true, true);
                sctx.LineTo(new Point(currentX, currentY -= 12), true, true);
                sctx.Close();
            }
            ctx.DrawGeometry(null, ResourceManager.DropRectPen, stream);

            //绘制方向箭头
            stream = new StreamGeometry();
            using (var sctx = stream.Open())
            {
                double currentX = hoffset + (Constants.DropUnitLength - Constants.DropGlassLength * 4) / 2, currentY = voffset - 20;
                sctx.BeginFigure(new Point(currentX, currentY), true, true);
                sctx.LineTo(new Point(currentX += 5, currentY -= 5), true, true);
                sctx.LineTo(new Point(currentX -= 10, currentY), true, true);
                sctx.Close();
            }
            ctx.DrawGeometry(Brushes.Black, null, stream);
        }

        private void _DrawCenter(DrawingContext ctx, double hoffset, double voffset, bool withSpliterLine = false, bool isVertical = false)
        {
            hoffset += DropPanel.Hoffset;
            voffset += +DropPanel.Voffset;
            double currentX = hoffset, currentY = voffset;

            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 2);

            ctx.DrawRectangle(Brushes.White, null, new Rect(currentX += Constants.DropGlassLength, currentY += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2));
            currentX += Constants.DropGlassLength;
            currentY += Constants.DropGlassLength + 2;

            ctx.DrawLine(ResourceManager.DropRectPen_Heavy, new Point(currentX - 0.5, currentY), new Point(currentX + Constants.DropUnitLength - Constants.DropGlassLength * 4 + 0.5, currentY));

            //绘制小窗口
            StreamGeometry stream = new StreamGeometry();
            using (var sctx = stream.Open())
            {
                sctx.BeginFigure(new Point(currentX, currentY), true, false);
                sctx.LineTo(new Point(currentX, currentY += 22), true, true);
                sctx.LineTo(new Point(currentX += Constants.DropUnitLength - Constants.DropGlassLength * 4, currentY), true, true);
                sctx.LineTo(new Point(currentX, currentY -= 22), true, true);
                sctx.Close();
            }
            ctx.DrawGeometry(null, ResourceManager.DropRectPen, stream);

            if (withSpliterLine)
            {
                if (isVertical)
                    ctx.DrawLine(ResourceManager.BlueDashPen, new Point(hoffset + Constants.DropUnitLength / 2, voffset + Constants.DropGlassLength * 2 + 3), new Point(hoffset + Constants.DropUnitLength / 2, voffset + Constants.DropUnitLength - 2 * Constants.DropGlassLength));
                else ctx.DrawLine(ResourceManager.BlueDashPen, new Point(hoffset + Constants.DropGlassLength * 2, voffset + Constants.DropUnitLength / 2), new Point(hoffset + Constants.DropUnitLength - 2 * Constants.DropGlassLength, voffset + Constants.DropUnitLength / 2));
            }
        }
    }
}