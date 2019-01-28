using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using YDock.Enum;

namespace YDock.Interface
{
    public interface INotifyDisposable
    {
        event EventHandler Disposed;
    }
    public interface IDockModel : ILayout, IDisposable
    {
        IDockView View { get; }
    }

    public interface IDockElement : IDockOrigin
    {
        XElement Save();
        void Load(XElement ele);
        void ToDockSide(DockSide side, bool isActive = false);
    }

    public interface IDockOrigin : ILayout, ILayoutSize, IDockItem, INotifyPropertyChanged, IDisposable
    {
        int ID { get; }
        string Title { get; set; }
        ImageSource ImageSource { get; }
        object Content { get; }
        bool IsDocument { get; }
        DockMode Mode { get; }
        bool IsVisible { get; }
        bool IsActive { get; }
        bool CanSelect { get; }
        bool IsDocked { get; }
        bool IsFloat { get; }
        bool IsAutoHide { get; }
        ILayoutGroup Container { get; }
    }

    public interface IDockControl : IDockOrigin
    {
        IDockElement ProtoType { get; }
        void Show(bool toActice = true);
        void Close();
        void SetActive(bool _isActive = true);
    }

    public interface IDockItem
    {
        bool IsDisposed { get; }
        void ToFloat(bool isActive = true);
        void ToDock(bool isActive = true);
        void ToDockAsDocument(bool isActive = true);
        void ToDockAsDocument(int index, bool isActive = true);
        void SwitchAutoHideStatus();
        void Hide();
        bool CanFloat { get; }
        bool CanDock { get; }
        bool CanDockAsDocument { get; }
        bool CanSwitchAutoHideStatus { get; }
        bool CanHide { get; }
    }
}