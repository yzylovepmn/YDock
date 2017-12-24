using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDock.Enum;

namespace YDock.Interface
{
    public interface IModel
    {
        IView View { get; }
    }
    public interface IAnchorModel : IModel
    {
        DockSide Side { get; }
    }
}
