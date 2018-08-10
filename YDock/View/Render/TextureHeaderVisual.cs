using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class TextureHeaderVisual : BaseVisual
    {
        public override void Update(Size size)
        {
            double voffset = (size.Height - 4) / 2;
            using (var ctx = RenderOpen())
            {
                var model = (VisualParent as FrameworkElement).DataContext as DockElement;
                if (model == null) return;
                if (model.IsActive)
                {
                    ctx.DrawLine(ResourceManager.ActiveDashPen, new Point(0, voffset), new Point(size.Width, voffset));
                    ctx.DrawLine(ResourceManager.ActiveDashPen, new Point(2, voffset + 2), new Point(size.Width, voffset + 2));
                    ctx.DrawLine(ResourceManager.ActiveDashPen, new Point(0, voffset + 4), new Point(size.Width, voffset + 4));
                }
                else
                {
                    ctx.DrawLine(ResourceManager.DisActiveDashPen, new Point(0, voffset), new Point(size.Width, voffset));
                    ctx.DrawLine(ResourceManager.DisActiveDashPen, new Point(2, voffset + 2), new Point(size.Width, voffset + 2));
                    ctx.DrawLine(ResourceManager.DisActiveDashPen, new Point(0, voffset + 4), new Point(size.Width, voffset + 4));
                }
            }
        }
    }
}