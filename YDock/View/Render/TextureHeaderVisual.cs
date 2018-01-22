using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace YDock.View
{
    public class TextureHeaderVisual : BaseVisual
    {
        public override void Update(Size size)
        {
            double voffset = (size.Height - 4) / 2;
            using (var ctx = RenderOpen())
            {
                ctx.DrawLine(ResourceManager.DashPen, new Point(0, voffset), new Point(size.Width, voffset));
                ctx.DrawLine(ResourceManager.DashPen, new Point(2, voffset + 2), new Point(size.Width, voffset + 2));
                ctx.DrawLine(ResourceManager.DashPen, new Point(0, voffset + 4), new Point(size.Width, voffset + 4));
            }
        }
    }
}