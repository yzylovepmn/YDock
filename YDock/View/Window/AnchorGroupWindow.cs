using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;
using System.Windows.Input;
using System.Windows.Controls;

namespace YDock.View
{
    public class AnchorGroupWindow : BaseFloatWindow, INotifyPropertyChanged
    {
        static AnchorGroupWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorGroupWindow), new FrameworkPropertyMetadata(typeof(AnchorGroupWindow)));
        }

        public AnchorGroupWindow(DockManager dockManager) : base(dockManager)
        {
            ShowInTaskbar = false;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// 是否Content为<see cref="ILayoutGroupControl"/>
        /// </summary>
        public bool IsSingleMode
        {
            get
            {
                return Content != null && Content is ILayoutGroupControl;
            }
        }

        /// <summary>
        /// 是否需要Border
        /// </summary>
        public bool NoBorder
        {
            get
            {
                return IsSingleMode && (Content as BaseGroupControl).Items.Count == 1;
            }
        }

        public override void AttachChild(IDockView child, AttachMode mode, int index)
        {
            if (child is ILayoutPanel)
                _heightEceeed += Constants.FloatWindowHeaderHeight;
            Owner = DockManager.MainWindow;
            base.AttachChild(child, mode, index);
            if (child is BaseGroupControl)
                (((child as BaseGroupControl).Model as BaseLayoutGroup).Children as ObservableCollection<IDockElement>).CollectionChanged += OnCollectionChanged;
        }

        public override void DetachChild(IDockView child, bool force = true)
        {
            if (child is BaseGroupControl)
                (((child as BaseGroupControl).Model as BaseLayoutGroup).Children as ObservableCollection<IDockElement>).CollectionChanged -= OnCollectionChanged;

            if (force)
                Owner = null;

            base.DetachChild(child, force);
            UpdateSize();
        }

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTemplate();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (newContent != null)
                UpdateTemplate();
        }

        protected override IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32Helper.WM_NCLBUTTONDOWN:
                case Win32Helper.WM_NCRBUTTONDOWN:
                    ActiveSelf();
                    if (IsSingleMode && msg == Win32Helper.WM_NCRBUTTONDOWN)
                        ContextMenu = new DockMenu((Child as BaseGroupControl).SelectedItem as IDockItem);
                    break;
            }
            return base.FilterMessage(hwnd, msg, wParam, lParam, ref handled);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            ContextMenu = null;
            base.OnMouseRightButtonUp(e);
        }

        public override void Recreate()
        {
            if (_needReCreate)
            {
                NeedReCreate = false;
                if (Child != null)
                {
                    var layoutCtrl = Child as BaseGroupControl;
                    layoutCtrl.IsDraggingFromDock = false;
                }
            }
        }

        public void UpdateTemplate()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("IsSingleMode"));
            PropertyChanged(this, new PropertyChangedEventArgs("NoBorder"));
            CommandManager.InvalidateRequerySuggested();
        }

        public void UpdateSize()
        {
            if (IsSingleMode)
                _heightEceeed = 0;
            else _heightEceeed = Constants.FloatWindowHeaderHeight;
        }

        protected override void OnMaximizeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((e.OriginalSource as Button)?.Name == "Maximize")
                e.CanExecute = IsSingleMode && WindowState == WindowState.Normal;
            else e.CanExecute = true;
        }

        protected override void OnRestoreCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((e.OriginalSource as Button)?.Name == "Restore")
                e.CanExecute = IsSingleMode && WindowState == WindowState.Maximized;
            else e.CanExecute = true;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            CommandManager.InvalidateRequerySuggested();
        }

        private void ActiveSelf()
        {
            if (IsSingleMode)
                ((Content as ILayoutGroupControl).Model as ILayoutGroup).ShowWithActice((Content as BaseGroupControl).SelectedIndex);
            DockManager.MainWindow.Activate();
        }
    }
}