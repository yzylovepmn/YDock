using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace YDock.View
{
    public class PopupNotTop : Popup
    {
        public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(typeof(PopupNotTop), new FrameworkPropertyMetadata(false, OnTopmostChanged));
        public bool Topmost
        {
            get
            {
                return (bool)GetValue(TopmostProperty);
            }
            set
            {
                SetValue(TopmostProperty, value);
            }
        }
        private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as PopupNotTop).UpdateWindow();
        }
        protected override void OnOpened(EventArgs e)
        {
            UpdateWindow();
        }
        private void UpdateWindow()
        {
            var hwnd = ((HwndSource)PresentationSource.FromDependencyObject(this)).Handle;
            RECT rect;
            if (GetWindowRect(hwnd, out rect))
            {
                SetWindowPos(hwnd, Topmost ? -1 : -2, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, 0);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);
    }
}
