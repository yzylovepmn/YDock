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

    }

    public interface IDockOrigin : ILayout, ILayoutSize, IDockItem, INotifyPropertyChanged, IDisposable
    {
        int ID { get; }
        string Title { get; set; }
        ImageSource ImageSource { get; }
        UIElement Content { get; }
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
        void Show();
        void Close();
        void SetActive(bool _isActive = true);
    }

    public interface IDockItem
    {
        bool IsDisposed { get; }
        void ToFloat();
        void ToDock();
        void ToDockAsDocument();
        void SwitchAutoHideStatus();
        void Hide();
        bool CanFloat { get; }
        bool CanDock { get; }
        bool CanDockAsDocument { get; }
        bool CanSwitchAutoHideStatus { get; }
        bool CanHide { get; }
    }
}