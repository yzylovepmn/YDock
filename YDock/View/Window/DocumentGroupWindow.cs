using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using YDock.Commands;
using YDock.Enum;
using YDock.Interface;

namespace YDock.View
{
    public class DocumentGroupWindow : BaseFloatWindow
    {
        static DocumentGroupWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentGroupWindow), new FrameworkPropertyMetadata(typeof(DocumentGroupWindow)));
        }

        public DocumentGroupWindow(DockManager dockManager) : base(dockManager)
        {

        }

        protected override void OnInitialized(EventArgs e)
        {
            CommandBindings.Add(new CommandBinding(GlobalCommands.MinimizeCommand, OnMinimizeExecute, OnMinimizeCanExecute));
            base.OnInitialized(e);
        }

        private void OnMinimizeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnMinimizeExecute(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public override void AttachChild(IDockView child, AttachMode mode, int index)
        {
            //后面的2是border Thickness
            _widthEceeed += Constants.DocumentWindowPadding * 2 + 2;
            _heightEceeed += Constants.DocumentWindowPadding * 2 + Constants.FloatWindowHeaderHeight + 2;
            base.AttachChild(child, mode, index);
        }

        public override void Recreate()
        {
            if (Child == null) return;
            if (_needReCreate)
            {
                _needReCreate = false;
                var layoutCtrl = Child as BaseGroupControl;
                layoutCtrl.BorderThickness = new Thickness(1, 0, 1, 1);
                layoutCtrl.IsDraggingFromDock = false;
                Background = ResourceManager.SplitterBrushVertical;
                BorderBrush = ResourceManager.SplitterBrushHorizontal;
            }
            else
            {
                _needReCreate = true;
                Background = Brushes.Transparent;
                BorderBrush = Brushes.Transparent;
                var layoutCtrl = Child as BaseGroupControl;
                layoutCtrl.BorderThickness = new Thickness(1);
                layoutCtrl.IsDraggingFromDock = true;
            }
        }
    }
}