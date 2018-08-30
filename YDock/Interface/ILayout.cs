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

    public interface ILayoutViewParent
    {
        void DetachChild(IDockView child, bool force = true);
        void AttachChild(IDockView child, AttachMode mode, int index);
        int IndexOf(IDockView child);
    }

    public interface ILayoutViewWithSize : ILayoutSize, IDockView
    {

    }

    public interface ILayoutPanel : ILayoutViewWithSize, ILayout, ILayoutViewParent, INotifyDisposable
    {
        int Count { get; }
        bool IsAnchorPanel { get; }
        bool IsDocumentPanel { get; }
    }

    public interface ILayoutSize
    {
        double DesiredWidth { get; set; }
        double DesiredHeight { get; set; }
        double FloatLeft { get; set; }
        double FloatTop { get; set; }
    }

    public interface ILayoutGroup : IDockModel, INotifyPropertyChanged
    {
        DockMode Mode { get; }
        IEnumerable<IDockElement> Children { get; }
        void MoveTo(int src, int des);
        int IndexOf(IDockElement child);
        void ShowWithActice(IDockElement element, bool toActice = true);
        void ShowWithActice(int index, bool toActice = true);
        void Detach(IDockElement element);
        void Attach(IDockElement element, int index = -1);
        void RaisePropertyChanged(string propertyName);
        void CloseAll();
        void CloseAllExcept(IDockElement element);
        void ToFloat();
    }

    public interface ILayoutGroupControl : ILayoutViewWithSize, INotifyDisposable, INotifyPropertyChanged
    {
        bool TryDeatchFromParent(bool isDispose = true);
        void AttachToParent(ILayoutPanel parent, int index);
    }
}