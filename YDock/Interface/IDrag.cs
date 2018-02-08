using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YDock.Enum;

namespace YDock.Interface
{
    public interface IDragTarget
    {
        bool IsDragWndHide { get; }
        DragMode Mode { get; }
        void OnDrop(DragItem source, int flag);
        void HideDropWindow();
        void ShowDropWindow();
        void CreateDropWindow();
        void CloseDropWindow();
    }
}