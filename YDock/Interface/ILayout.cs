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
        IEnumerable<IDockElement> Children { get; }
        void MoveTo(int src, int des);
        int IndexOf(IDockElement child);
        void RaisePropertyChanged(string propertyName);
    }

    public interface ILayoutGroupControl : ILayoutSize, IDockView
    {

    }
}