using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDock.Enum;

namespace YDock.Interface
{
    public interface ILayout
    {
        DockSide Side { get; }
        YDock DockManager { get; }
    }

    public interface ILayoutSize
    {
        double DesiredWidth { get; set; }
        double DesiredHeight { get; set; }
    }

    public interface ILayoutGroup : ILayout, IModel
    {
        IEnumerable<ILayoutElement> Children { get; }
    }

    public interface ILayoutGroupControl : ILayoutSize, IView
    {

    }
}