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
                    hoffeset = size.Width / 2 - Constants.DropUnitLength * 5 / 2;
                    voffeset = size.Height / 2 - Constants.DropUnitLength / 2;
                    sideLength = Constants.DropUnitLength * 2 - Constants.DropCornerLength;
                }
                else if(DropPanel.Target.Mode == DragMode.None || DropPanel.Source.DragMode == DragMode.None || (DropPanel.Target.Mode == DragMode.Anchor
                    && DropPanel.Source.DragMode == DragMode.Document))
                    return;
                else
                {
                    hoffeset = size.Width / 2 - Constants.DropUnitLength * 3 / 2;
                    voffeset = size.Height / 2 - Constants.DropUnitLength / 2;
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
                if (DropPanel is RootDropPanel)
                {
                    double hoffeset = 0, voffeset = 0;
                    if ((Flag & DragManager.LEFT) != 0)
                    {
                        hoffeset = Constants.DropGlassLength;
                        voffeset = (size.Height - Constants.DropUnitLength) / 2;
                        //绘制玻璃外观
                        ctx.PushOpacity(Constants.DragOpacity);
                        ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffeset, voffeset, Constants.DropUnitLength, Constants.DropUnitLength));
                        ctx.Pop();
                        if ((Flag & DragManager.ACTIVE) == 0)
                            ctx.PushOpacity(Constants.DragOpacity * 2);
                        ctx.DrawRectangle(Brushes.White, null, new Rect(hoffeset += Constants.DropGlassLength, voffeset += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2));
                        hoffeset += Constants.DropGlassLength;
                        voffeset += Constants.DropGlassLength;
                        ctx.DrawLine(ResourceManager.DropRectPen_Heavy, new Point(hoffeset - 0.5, voffeset), new Point(hoffeset + 12.5, voffeset));

                        //绘制小窗口
                        StreamGeometry stream = new StreamGeometry();
                        using (var sctx = stream.Open())
                        {
                            double currentX = hoffeset, currentY = voffeset;
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
                            double currentX = hoffeset + 20, currentY = voffeset + (Constants.DropUnitLength - Constants.DropGlassLength * 4) / 2;
                            sctx.BeginFigure(new Point(currentX, currentY), true, true);
                            sctx.LineTo(new Point(currentX += 5, currentY -= 5), true, true);
                            sctx.LineTo(new Point(currentX, currentY += 10), true, true);
                            sctx.Close();
                        }
                        ctx.DrawGeometry(Brushes.Black, null, stream);
                    }
                    else if ((Flag & DragManager.TOP) != 0)
                    {
                        hoffeset = (size.Width - Constants.DropUnitLength) / 2;
                        voffeset = Constants.DropGlassLength;
                        //绘制玻璃外观
                        ctx.PushOpacity(Constants.DragOpacity);
                        ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffeset, voffeset, Constants.DropUnitLength, Constants.DropUnitLength));
                        ctx.Pop();
                        if ((Flag & DragManager.ACTIVE) == 0)
                            ctx.PushOpacity(Constants.DragOpacity * 2);

                        ctx.DrawRectangle(Brushes.White, null, new Rect(hoffeset += Constants.DropGlassLength, voffeset += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2));
                        hoffeset += Constants.DropGlassLength;
                        voffeset += Constants.DropGlassLength;

                        ctx.DrawLine(ResourceManager.DropRectPen_Heavy, new Point(hoffeset - 0.5, voffeset), new Point(hoffeset + Constants.DropUnitLength - Constants.DropGlassLength * 4 + 0.5, voffeset));

                        //绘制小窗口
                        StreamGeometry stream = new StreamGeometry();
                        using (var sctx = stream.Open())
                        {
                            double currentX = hoffeset, currentY = voffeset;
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
                            double currentX = hoffeset + (Constants.DropUnitLength - Constants.DropGlassLength * 4) / 2, currentY = voffeset + 20;
                            sctx.BeginFigure(new Point(currentX, currentY), true, true);
                            sctx.LineTo(new Point(currentX += 5, currentY += 5), true, true);
                            sctx.LineTo(new Point(currentX -= 10, currentY), true, true);
                            sctx.Close();
                        }
                        ctx.DrawGeometry(Brushes.Black, null, stream);
                    }
                    else if ((Flag & DragManager.RIGHT) != 0)
                    {
                        hoffeset = size.Width - Constants.DropGlassLength;
                        voffeset = (size.Height - Constants.DropUnitLength) / 2;
                        //绘制玻璃外观
                        ctx.PushOpacity(Constants.DragOpacity);
                        ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffeset - Constants.DropUnitLength, voffeset, Constants.DropUnitLength, Constants.DropUnitLength));
                        ctx.Pop();
                        if ((Flag & DragManager.ACTIVE) == 0)
                            ctx.PushOpacity(Constants.DragOpacity * 2);
                        ctx.DrawRectangle(Brushes.White, null, new Rect((hoffeset -= Constants.DropGlassLength) - (Constants.DropUnitLength - Constants.DropGlassLength * 2), voffeset += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2));
                        hoffeset -= Constants.DropGlassLength;
                        voffeset += Constants.DropGlassLength;
                        ctx.DrawLine(ResourceManager.DropRectPen_Heavy, new Point(hoffeset + 0.5, voffeset), new Point(hoffeset - 12.5, voffeset));

                        //绘制小窗口
                        StreamGeometry stream = new StreamGeometry();
                        using (var sctx = stream.Open())
                        {
                            double currentX = hoffeset, currentY = voffeset;
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
                            double currentX = hoffeset - 20, currentY = voffeset + (Constants.DropUnitLength - Constants.DropGlassLength * 4) / 2;
                            sctx.BeginFigure(new Point(currentX, currentY), true, true);
                            sctx.LineTo(new Point(currentX -= 5, currentY -= 5), true, true);
                            sctx.LineTo(new Point(currentX, currentY += 10), true, true);
                            sctx.Close();
                        }
                        ctx.DrawGeometry(Brushes.Black, null, stream);
                    }
                    else if ((Flag & DragManager.BOTTOM) != 0)
                    {
                        hoffeset = (size.Width - Constants.DropUnitLength) / 2;
                        voffeset = size.Height - Constants.DropGlassLength;
                        //绘制玻璃外观
                        ctx.PushOpacity(Constants.DragOpacity);
                        ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffeset, voffeset - Constants.DropUnitLength, Constants.DropUnitLength, Constants.DropUnitLength));
                        ctx.Pop();
                        if ((Flag & DragManager.ACTIVE) == 0)
                            ctx.PushOpacity(Constants.DragOpacity * 2);

                        ctx.DrawRectangle(Brushes.White, null, new Rect(hoffeset += Constants.DropGlassLength, (voffeset -= Constants.DropGlassLength) - (Constants.DropUnitLength - Constants.DropGlassLength * 2), Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2));
                        hoffeset += Constants.DropGlassLength;
                        voffeset -= Constants.DropGlassLength;

                        ctx.DrawLine(ResourceManager.DropRectPen_Heavy, new Point(hoffeset - 0.5, voffeset - 12), new Point(hoffeset + Constants.DropUnitLength - Constants.DropGlassLength * 4 + 0.5, voffeset - 12));

                        //绘制小窗口
                        StreamGeometry stream = new StreamGeometry();
                        using (var sctx = stream.Open())
                        {
                            double currentX = hoffeset, currentY = voffeset - 12;
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
                            double currentX = hoffeset + (Constants.DropUnitLength - Constants.DropGlassLength * 4) / 2, currentY = voffeset - 20;
                            sctx.BeginFigure(new Point(currentX, currentY), true, true);
                            sctx.LineTo(new Point(currentX += 5, currentY -= 5), true, true);
                            sctx.LineTo(new Point(currentX -= 10, currentY), true, true);
                            sctx.Close();
                        }
                        ctx.DrawGeometry(Brushes.Black, null, stream);
                    }
                }
                else
                {

                }
            }
        }
    }
}