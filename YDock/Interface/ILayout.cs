using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YDock.Enum;

namespace YDock.Interface
{
    public interface ILayout
    {
        DockSide Side { get; }
        DockManager DockManager { get; }
    }

    public interface ILayoutSize
    {
        double DesiredWidth { get; set; }
        double DesiredHeight { get; set; }
    }

    public interface ILayoutGroup : IDockModel, INotifyPropertyChanged
    {
        IEnumerable<ILayoutElement> Children { get; }
        void MoveTo(int src, int des);
        int IndexOf(ILayoutElement child);
        void RaisePropertyChanged(string propertyName);
    }

    public interface ILayoutGroupControl : ILayoutSize, IDockView
    {

    }
}