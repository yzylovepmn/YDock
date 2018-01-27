using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using YDock.Enum;

namespace YDock.Interface
{
    public interface IDockModel : ILayout, IDisposable
    {
        IDockView View { get; }
    }

    public interface IDockElement : IDockOrigin
    {
        ILayoutGroup Container { get; }
    }

    public interface IDockOrigin: ILayout, ILayoutSize, INotifyPropertyChanged, IDisposable
    {
        int ID { get; }
        string Title { get; }
        ImageSource ImageSource { get; }
        UIElement Content { get; }
        DockStatus Status { get; }
    }

    public interface IDockControl : IDockOrigin
    {

    }
}