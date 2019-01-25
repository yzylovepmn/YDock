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
                double hoffset = 0, voffset = 0, sideLength = 0;
                if (DropPanel.Target.Mode == DragMode.Document
                    && DropPanel.Source.DragMode == DragMode.Anchor)
                {
                    hoffset = DropPanel.InnerRect.Size.Width / 2 - Constants.DropUnitLength * 5 / 2 + DropPanel.InnerRect.Left + DropPanel.OuterRect.Left;
                    voffset = DropPanel.InnerRect.Size.Height / 2 - Constants.DropUnitLength / 2 + DropPanel.InnerRect.Top + DropPanel.OuterRect.Top;
                    sideLength = Constants.DropUnitLength * 2 - Constants.DropCornerLength;
                }
                else if(DropPanel.Target.Mode == DragMode.None || DropPanel.Source.DragMode == DragMode.None || (DropPanel.Target.Mode == DragMode.Anchor
                    && DropPanel.Source.DragMode == DragMode.Document))
                    return;
                else
                {
                    hoffset = DropPanel.InnerRect.Size.Width / 2 - Constants.DropUnitLength * 3 / 2 + DropPanel.InnerRect.Left + DropPanel.OuterRect.Left;
                    voffset = DropPanel.InnerRect.Size.Height / 2 - Constants.DropUnitLength / 2 + DropPanel.InnerRect.Top + DropPanel.OuterRect.Top;
                    sideLength = Constants.DropUnitLength - Constants.DropCornerLength;
                }

                ctx.PushOpacity(Constants.DragOpacity);
                List<Point> points = new List<Point>();
                double currentX = hoffset, currentY = voffset;

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
            using (var ctx = RenderOpen())
            {
                if (Flag != DragManager.NONE)
                {
                    ctx.PushOpacity(Constants.DragOpacity);
                    if (Rect.IsEmpty)
                    {
                        if (DropPanel.Target is LayoutDocumentGroupControl)
                        {
                            double innerLeft = DropPanel.InnerRect.Left + DropPanel.OuterRect.Left;
                            double innerTop = DropPanel.InnerRect.Top + DropPanel.OuterRect.Top;
                            if ((Flag & DragManager.LEFT) != 0)
                            {
                                if ((Flag & DragManager.SPLIT) != 0)
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(innerLeft, innerTop, DropPanel.InnerRect.Size.Width / 2, DropPanel.InnerRect.Size.Height));
                                else ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.OuterRect.Left, DropPanel.OuterRect.Top, Math.Min(DropPanel.OuterRect.Size.Width / 2, DropPanel.Source.Size.Width), DropPanel.OuterRect.Size.Height));
                            }
                            if ((Flag & DragManager.TOP) != 0)
                            {
                                if ((Flag & DragManager.SPLIT) != 0)
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(innerLeft, innerTop, DropPanel.InnerRect.Size.Width, DropPanel.InnerRect.Size.Height / 2));
                                else ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.OuterRect.Left, DropPanel.OuterRect.Top, DropPanel.OuterRect.Size.Width, Math.Min(DropPanel.OuterRect.Size.Height / 2, DropPanel.Source.Size.Height)));
                            }
                            if ((Flag & DragManager.RIGHT) != 0)
                            {
                                if ((Flag & DragManager.SPLIT) != 0)
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(innerLeft + DropPanel.InnerRect.Size.Width / 2, innerTop, DropPanel.InnerRect.Size.Width / 2, DropPanel.InnerRect.Size.Height));
                                else
                                {
                                    double length = Math.Min(DropPanel.OuterRect.Size.Width / 2, DropPanel.Source.Size.Width);
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.OuterRect.Left + DropPanel.OuterRect.Size.Width - length, DropPanel.OuterRect.Top, length, DropPanel.OuterRect.Size.Height));
                                }
                            }
                            if ((Flag & DragManager.BOTTOM) != 0)
                            {
                                if ((Flag & DragManager.SPLIT) != 0)
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(innerLeft, innerTop + DropPanel.InnerRect.Size.Height / 2, DropPanel.InnerRect.Size.Width, DropPanel.InnerRect.Size.Height / 2));
                                else
                                {
                                    double length = Math.Min(DropPanel.OuterRect.Size.Height / 2, DropPanel.Source.Size.Height);
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.OuterRect.Left, DropPanel.OuterRect.Top + DropPanel.OuterRect.Size.Height - length, DropPanel.OuterRect.Size.Width, length));
                                }
                            }
                        }
                        else
                        {
                            if ((Flag & DragManager.LEFT) != 0)
                            {
                                if ((Flag & DragManager.SPLIT) != 0)
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.InnerRect.Left, DropPanel.InnerRect.Top, DropPanel.InnerRect.Size.Width / 2, DropPanel.InnerRect.Size.Height));
                                else ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.InnerRect.Left, DropPanel.InnerRect.Top, Math.Min(DropPanel.InnerRect.Size.Width / 2, DropPanel.Source.Size.Width), DropPanel.InnerRect.Size.Height));
                            }
                            if ((Flag & DragManager.TOP) != 0)
                            {
                                if ((Flag & DragManager.SPLIT) != 0)
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.InnerRect.Left, DropPanel.InnerRect.Top, DropPanel.InnerRect.Size.Width, DropPanel.InnerRect.Size.Height / 2));
                                else ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.InnerRect.Left, DropPanel.InnerRect.Top, DropPanel.InnerRect.Size.Width, Math.Min(DropPanel.InnerRect.Size.Height / 2, DropPanel.Source.Size.Height)));
                            }
                            if ((Flag & DragManager.RIGHT) != 0)
                            {
                                if ((Flag & DragManager.SPLIT) != 0)
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.InnerRect.Left + DropPanel.InnerRect.Size.Width / 2, DropPanel.InnerRect.Top, DropPanel.InnerRect.Size.Width / 2, DropPanel.InnerRect.Size.Height));
                                else
                                {
                                    double length = Math.Min(DropPanel.InnerRect.Size.Width / 2, DropPanel.Source.Size.Width);
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.InnerRect.Left + DropPanel.InnerRect.Size.Width - length, DropPanel.InnerRect.Top, length, DropPanel.InnerRect.Size.Height));
                                }
                            }
                            if ((Flag & DragManager.BOTTOM) != 0)
                            {
                                if ((Flag & DragManager.SPLIT) != 0)
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.InnerRect.Left, DropPanel.InnerRect.Top + DropPanel.InnerRect.Size.Height / 2, DropPanel.InnerRect.Size.Width, DropPanel.InnerRect.Size.Height / 2));
                                else
                                {
                                    double length = Math.Min(DropPanel.InnerRect.Size.Height / 2, DropPanel.Source.Size.Height);
                                    ctx.DrawRectangle(ResourceManager.RectBrush, ResourceManager.RectBorderPen, new Rect(DropPanel.InnerRect.Left, DropPanel.InnerRect.Top + DropPanel.InnerRect.Size.Height - length, DropPanel.InnerRect.Size.Width, length));
                                }
                            }
                        }
                        if ((Flag & DragManager.CENTER) != 0)
                        {
                            StreamGeometry stream = new StreamGeometry();
                            using (var sctx = stream.Open())
                            {
                                double currentX = DropPanel.InnerRect.Left + DropPanel.OuterRect.Left, currentY = DropPanel.InnerRect.Top + DropPanel.OuterRect.Top;
                                sctx.BeginFigure(new Point(currentX, currentY), true, false);
                                if (DropPanel.Target.Mode == DragMode.Anchor)
                                {
                                    sctx.LineTo(new Point(currentX += DropPanel.InnerRect.Size.Width, currentY), true, false);
                                    if (DropPanel.InnerRect.Size.Width < 60)
                                    {
                                        sctx.LineTo(new Point(currentX, currentY += DropPanel.InnerRect.Size.Height), true, false);
                                        sctx.LineTo(new Point(currentX -= DropPanel.InnerRect.Size.Width, currentY), true, false);
                                    }
                                    else
                                    {
                                        sctx.LineTo(new Point(currentX, currentY += DropPanel.InnerRect.Size.Height - 20), true, false);
                                        sctx.LineTo(new Point(currentX -= DropPanel.InnerRect.Size.Width - 60, currentY), true, false);
                                        sctx.LineTo(new Point(currentX, currentY += 20), true, false);
                                        sctx.LineTo(new Point(currentX -= 60, currentY), true, false);
                                    }
                                    sctx.LineTo(new Point(currentX, currentY -= DropPanel.InnerRect.Size.Height), true, false);
                                }
                                else
                                {
                                    if (DropPanel.InnerRect.Size.Width < 120)
                                    {
                                        sctx.LineTo(new Point(currentX += DropPanel.InnerRect.Size.Width, currentY), true, false);
                                        sctx.LineTo(new Point(currentX, currentY += DropPanel.InnerRect.Size.Height), true, false);
                                    }
                                    else
                                    {
                                        sctx.LineTo(new Point(currentX += 120, currentY), true, false);
                                        sctx.LineTo(new Point(currentX, currentY += 22), true, false);
                                        sctx.LineTo(new Point(currentX += DropPanel.InnerRect.Size.Width - 120, currentY), true, false);
                                        sctx.LineTo(new Point(currentX, currentY += DropPanel.InnerRect.Size.Height - 22), true, false);
                                    }
                                    sctx.LineTo(new Point(currentX -= DropPanel.InnerRect.Size.Width, currentY), true, false);
                                    sctx.LineTo(new Point(currentX, currentY -= DropPanel.InnerRect.Size.Height), true, false);
                                }
                                sctx.Close();
                            }
                            ctx.DrawGeometry(ResourceManager.RectBrush, ResourceManager.RectBorderPen, stream);
                        }
                    }
                    else
                    {
                        StreamGeometry stream = new StreamGeometry();
                        using (var sctx = stream.Open())
                        {
                            double currentX = DropPanel.InnerRect.Left + DropPanel.OuterRect.Left, currentY = DropPanel.InnerRect.Top + DropPanel.OuterRect.Top;
                            if (DropPanel.Target.Mode == DragMode.Anchor)
                            {
                                sctx.BeginFigure(new Point(currentX, currentY), true, false);
                                if (DropPanel.InnerRect.Size.Width < Rect.X + Rect.Width)
                                    Rect.Width = DropPanel.InnerRect.Size.Width - Rect.X;
                                sctx.LineTo(new Point(currentX, currentY += DropPanel.InnerRect.Size.Height - 20), true, false);
                                sctx.LineTo(new Point(currentX += Rect.X, currentY), true, false);
                                sctx.LineTo(new Point(currentX, currentY += 20), true, false);
                                sctx.LineTo(new Point(currentX += Rect.Width, currentY), true, false);
                                sctx.LineTo(new Point(currentX, currentY -= 20), true, false);
                                sctx.LineTo(new Point(currentX += DropPanel.InnerRect.Size.Width - Rect.Width - Rect.X, currentY), true, false);
                                sctx.LineTo(new Point(currentX, currentY -= DropPanel.InnerRect.Size.Height - 20), true, false);
                                sctx.LineTo(new Point(currentX -= DropPanel.InnerRect.Size.Width, currentY), true, false);
                            }
                            else
                            {
                                currentY += DropPanel.InnerRect.Size.Height;
                                sctx.BeginFigure(new Point(currentX, currentY), true, false);
                                if (DropPanel.InnerRect.Size.Width < Rect.X + Rect.Width)
                                    Rect.Width = DropPanel.InnerRect.Size.Width - Rect.X;
                                sctx.LineTo(new Point(currentX, currentY -= (DropPanel.InnerRect.Size.Height - 22)), true, false);
                                sctx.LineTo(new Point(currentX += Rect.X, currentY), true, false);
                                sctx.LineTo(new Point(currentX, currentY -= 22), true, false);
                                sctx.LineTo(new Point(currentX += Rect.Width, currentY), true, false);
                                sctx.LineTo(new Point(currentX, currentY += 22), true, false);
                                sctx.LineTo(new Point(currentX += (DropPanel.InnerRect.Size.Width - Rect.Width - Rect.X), currentY), true, false);
                                sctx.LineTo(new Point(currentX, currentY += (DropPanel.InnerRect.Size.Height - 22)), true, false);
                                sctx.LineTo(new Point(currentX -= DropPanel.InnerRect.Size.Width, currentY), true, false);
                            }
                            sctx.Close();
                        }
                        ctx.DrawGeometry(ResourceManager.RectBrush, ResourceManager.RectBorderPen, stream);
                    }
                }
            }
        }

        internal Rect Rect;
    }

    public class UnitDropVisual: BaseDropVisual
    {
        internal UnitDropVisual(int flag) : base(flag)
        {

        }

        public override void Update(Size size)
        {
            if (DropPanel.Target == null) return;
            using (var ctx = RenderOpen())
            {
                double hoffset = DropPanel.InnerRect.Left + DropPanel.OuterRect.Left, voffset = DropPanel.InnerRect.Top + DropPanel.OuterRect.Top;
                if (DropPanel is RootDropPanel)
                {
                    if ((Flag & DragManager.LEFT) != 0)
                        _DrawLeft(ctx, hoffset + Constants.DropGlassLength, voffset + (size.Height - Constants.DropUnitLength) / 2);
                    else if ((Flag & DragManager.TOP) != 0)
                        _DrawTop(ctx, hoffset + (size.Width - Constants.DropUnitLength) / 2, voffset + Constants.DropGlassLength);
                    else if ((Flag & DragManager.RIGHT) != 0)
                        _DrawRight(ctx, hoffset + size.Width - Constants.DropGlassLength, voffset + (size.Height - Constants.DropUnitLength) / 2);
                    else if ((Flag & DragManager.BOTTOM) != 0)
                        _DrawBottom(ctx, hoffset + (size.Width - Constants.DropUnitLength) / 2, voffset + size.Height - Constants.DropGlassLength);
                }
                else
                {
                    bool flag = false;
                    if ((Flag & DragManager.CENTER) != 0)
                    {
                        if (DropPanel.Target.Mode == DragMode.Document)
                            flag = true;
                        if (DropPanel.Source.DragMode != DragMode.Document
                            && DropPanel.Target.Mode == DragMode.Anchor)
                            flag = true;

                        if (flag)
                            _DrawCenter(ctx, hoffset + (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2, voffset + (DropPanel.InnerRect.Size.Height - Constants.DropUnitLength) / 2);
                    }

                    flag = true;
                    LayoutDocumentGroupControl layoutCrtl;
                    LayoutGroupPanel layoutpanel;

                    if ((Flag & DragManager.LEFT) != 0)
                    {
                        if ((Flag & DragManager.SPLIT) != 0)
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                layoutCrtl = DropPanel.Target as LayoutDocumentGroupControl;
                                if (layoutCrtl.DockViewParent != null)
                                {
                                    layoutpanel = layoutCrtl.DockViewParent as LayoutGroupPanel;
                                    flag &= layoutpanel.Direction != Direction.Vertical && layoutCrtl.ChildrenCount > 0;
                                }
                                if (flag)
                                {
                                    hoffset += (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2 - Constants.DropUnitLength;
                                    voffset += (DropPanel.InnerRect.Size.Height - Constants.DropUnitLength) / 2;
                                    _DrawCenter(ctx, hoffset, voffset, true, true);
                                }
                            }
                        }
                        else
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                layoutCrtl = DropPanel.Target as LayoutDocumentGroupControl;
                                if (layoutCrtl.DockViewParent != null)
                                {
                                    layoutpanel = layoutCrtl.DockViewParent as LayoutGroupPanel;
                                    if (layoutpanel.Direction == Direction.Horizontal)
                                        flag &= layoutCrtl.IndexOf() == 0;
                                }
                                hoffset += (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2 - Constants.DropUnitLength * 2;
                                voffset += (DropPanel.InnerRect.Size.Height - Constants.DropUnitLength) / 2;
                            }
                            else
                            {
                                hoffset += (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2 - Constants.DropUnitLength;
                                voffset += (DropPanel.InnerRect.Size.Height - Constants.DropUnitLength) / 2;
                            }
                            if (DropPanel.Source.DragMode == DragMode.Anchor && flag)
                                _DrawLeft(ctx, hoffset, voffset, false);
                        }
                    }

                    if ((Flag & DragManager.RIGHT) != 0)
                    {
                        if ((Flag & DragManager.SPLIT) != 0)
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                layoutCrtl = DropPanel.Target as LayoutDocumentGroupControl;
                                if (layoutCrtl.DockViewParent != null)
                                {
                                    layoutpanel = layoutCrtl.DockViewParent as LayoutGroupPanel;
                                    flag &= layoutpanel.Direction != Direction.Vertical && layoutCrtl.ChildrenCount > 0;
                                }
                                if (flag)
                                {
                                    hoffset += (DropPanel.InnerRect.Size.Width + Constants.DropUnitLength) / 2;
                                    voffset += (DropPanel.InnerRect.Size.Height - Constants.DropUnitLength) / 2;
                                    _DrawCenter(ctx, hoffset, voffset, true, true);
                                }
                            }
                        }
                        else
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                layoutCrtl = DropPanel.Target as LayoutDocumentGroupControl;
                                if (layoutCrtl.DockViewParent != null)
                                {
                                    layoutpanel = layoutCrtl.DockViewParent as LayoutGroupPanel;
                                    if (layoutpanel.Direction == Direction.Horizontal)
                                        flag &= layoutCrtl.IndexOf() == layoutpanel.Count - 1;
                                }
                                hoffset += DropPanel.InnerRect.Size.Width / 2 + Constants.DropUnitLength * 2.5;
                                voffset += (DropPanel.InnerRect.Size.Height - Constants.DropUnitLength) / 2;
                            }
                            else
                            {
                                hoffset += DropPanel.InnerRect.Size.Width / 2 + Constants.DropUnitLength * 1.5;
                                voffset += (DropPanel.InnerRect.Size.Height - Constants.DropUnitLength) / 2;
                            }
                            if (DropPanel.Source.DragMode == DragMode.Anchor && flag)
                                _DrawRight(ctx, hoffset, voffset, false);
                        }
                    }

                    if ((Flag & DragManager.TOP) != 0)
                    {
                        if ((Flag & DragManager.SPLIT) != 0)
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                layoutCrtl = DropPanel.Target as LayoutDocumentGroupControl;
                                if (layoutCrtl.DockViewParent != null)
                                {
                                    layoutpanel = layoutCrtl.DockViewParent as LayoutGroupPanel;
                                    flag &= layoutpanel.Direction != Direction.Horizontal && layoutCrtl.ChildrenCount > 0;
                                }
                                if (flag)
                                {
                                    hoffset += (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2;
                                    voffset += DropPanel.InnerRect.Size.Height / 2 - Constants.DropUnitLength * 1.5;
                                    _DrawCenter(ctx, hoffset, voffset, true);
                                }
                            }
                        }
                        else
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                layoutCrtl = DropPanel.Target as LayoutDocumentGroupControl;
                                if (layoutCrtl.DockViewParent != null)
                                {
                                    layoutpanel = layoutCrtl.DockViewParent as LayoutGroupPanel;
                                    if (layoutpanel.Direction == Direction.Vertical)
                                        flag &= layoutCrtl.IndexOf() == 0;
                                }
                                hoffset += (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2;
                                voffset += DropPanel.InnerRect.Size.Height / 2 - Constants.DropUnitLength * 2.5;
                            }
                            else
                            {
                                hoffset += (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2;
                                voffset += DropPanel.InnerRect.Size.Height / 2 - Constants.DropUnitLength * 1.5;
                            }
                            if (DropPanel.Source.DragMode == DragMode.Anchor && flag)
                                _DrawTop(ctx, hoffset, voffset, false);
                        }
                    }

                    if ((Flag & DragManager.BOTTOM) != 0)
                    {
                        if ((Flag & DragManager.SPLIT) != 0)
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                layoutCrtl = DropPanel.Target as LayoutDocumentGroupControl;
                                if (layoutCrtl.DockViewParent != null)
                                {
                                    layoutpanel = layoutCrtl.DockViewParent as LayoutGroupPanel;
                                    flag &= layoutpanel.Direction != Direction.Horizontal && layoutCrtl.ChildrenCount > 0;
                                }
                                if (flag)
                                {
                                    hoffset += (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2;
                                    voffset += (DropPanel.InnerRect.Size.Height + Constants.DropUnitLength) / 2;
                                    _DrawCenter(ctx, hoffset, voffset, true);
                                }
                            }
                        }
                        else
                        {
                            if (DropPanel.Target.Mode == DragMode.Document)
                            {
                                layoutCrtl = DropPanel.Target as LayoutDocumentGroupControl;
                                if (layoutCrtl.DockViewParent != null)
                                {
                                    layoutpanel = layoutCrtl.DockViewParent as LayoutGroupPanel;
                                    if (layoutpanel.Direction == Direction.Vertical)
                                        flag &= layoutCrtl.IndexOf() == layoutpanel.Count - 1;
                                }
                                hoffset += (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2;
                                voffset += DropPanel.InnerRect.Size.Height / 2 + Constants.DropUnitLength * 2.5;
                            }
                            else
                            {
                                hoffset += (DropPanel.InnerRect.Size.Width - Constants.DropUnitLength) / 2;
                                voffset += DropPanel.InnerRect.Size.Height / 2 + Constants.DropUnitLength * 1.5;
                            }
                            if (DropPanel.Source.DragMode == DragMode.Anchor && flag)
                                _DrawBottom(ctx, hoffset, voffset, false);
                        }
                    }
                }
            }
        }

        private void _DrawLeft(DrawingContext ctx, double hoffset, double voffset, bool hasGlassBorder = true)
        {
            if (hasGlassBorder)
            {
                //绘制玻璃外观
                ctx.PushOpacity(Constants.DragOpacity);
                ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffset, voffset, Constants.DropUnitLength, Constants.DropUnitLength));
                ctx.Pop();
            }
            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 1.8);
            ctx.DrawRoundedRectangle(Brushes.White, null, new Rect(hoffset += Constants.DropGlassLength, voffset += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2), 3, 3);
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
            if (hasGlassBorder)
            {
                //绘制玻璃外观
                ctx.PushOpacity(Constants.DragOpacity);
                ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffset, voffset, Constants.DropUnitLength, Constants.DropUnitLength));
                ctx.Pop();
            }
            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 1.8);

            ctx.DrawRoundedRectangle(Brushes.White, null, new Rect(hoffset += Constants.DropGlassLength, voffset += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2), 3, 3);
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
            if (hasGlassBorder)
            {
                //绘制玻璃外观
                ctx.PushOpacity(Constants.DragOpacity);
                ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffset - Constants.DropUnitLength, voffset, Constants.DropUnitLength, Constants.DropUnitLength));
                ctx.Pop();
            }

            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 1.8);
            ctx.DrawRoundedRectangle(Brushes.White, null, new Rect((hoffset -= Constants.DropGlassLength) - (Constants.DropUnitLength - Constants.DropGlassLength * 2), voffset += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2), 3, 3);
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
            if (hasGlassBorder)
            {
                //绘制玻璃外观
                ctx.PushOpacity(Constants.DragOpacity);
                ctx.DrawRectangle(Brushes.White, ResourceManager.BorderPen, new Rect(hoffset, voffset - Constants.DropUnitLength, Constants.DropUnitLength, Constants.DropUnitLength));
                ctx.Pop();
            }

            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 1.8);

            ctx.DrawRoundedRectangle(Brushes.White, null, new Rect(hoffset += Constants.DropGlassLength, (voffset -= Constants.DropGlassLength) - (Constants.DropUnitLength - Constants.DropGlassLength * 2), Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2), 3, 3);
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
            double currentX = hoffset, currentY = voffset;

            if ((Flag & DragManager.ACTIVE) == 0)
                ctx.PushOpacity(Constants.DragOpacity * 1.8);

            ctx.DrawRoundedRectangle(Brushes.White, null, new Rect(currentX += Constants.DropGlassLength, currentY += Constants.DropGlassLength, Constants.DropUnitLength - Constants.DropGlassLength * 2, Constants.DropUnitLength - Constants.DropGlassLength * 2), 3, 3);
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