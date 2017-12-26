using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YDock.Interface
{
    public interface ILayoutContainer
    {
        IEnumerable<ILayoutElement> Children { get; }
    }
}