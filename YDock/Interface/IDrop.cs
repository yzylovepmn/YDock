using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YDock.Enum;

namespace YDock.Interface
{
    public interface IDropWindow
    {
        void Hide();
        void Show();
        void Close();
        void Update(Point mouseP);
    }
}