using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YDock.Enum;

namespace YDock.Interface
{
    public interface IModel
    {
        IView View { get; set; }
    }
    public interface IAnchorModel : IModel
    {
        DockSide Side { get; }
    }

    public interface ILayoutElement : INotifyPropertyChanged
    {
        ILayoutContainer Container { get; set; }
        double ActualWidth { get; }
        double ActualHeight { get; }
    }
}