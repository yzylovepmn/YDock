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

    public interface ILayoutElement : INotifyPropertyChanged, ILayout, ILayoutSize
    {
        ILayoutGroup Container { get; }
    }
}