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

    public interface ILayoutPanel : ILayoutViewWithSize, ILayout, ILayoutViewParent
    {
        int Count { get; }
        bool IsAnchorPanel { get; }
        bool IsDocumentPanel { get; }
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
        void SetActive(IDockElement element);
        void SetActive(int index);
        void Detach(IDockElement element);
        void Attach(IDockElement element, int index = -1);
        void SetDockMode(DockMode mode);
        void RaisePropertyChanged(string propertyName);
    }

    public interface ILayoutGroupControl : ILayoutViewWithSize, INotifyPropertyChanged
    {
        bool TryDeatchFromParent(bool isDispose = true);
        void AttachToParent(ILayoutPanel parent, int index);
    }
}