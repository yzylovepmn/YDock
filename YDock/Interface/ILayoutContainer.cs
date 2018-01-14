using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace YDock.Interface
{
    public interface ILayoutContainer : INotifyPropertyChanged
    {
        IEnumerable<ILayoutElement> Children { get; }
    }
}