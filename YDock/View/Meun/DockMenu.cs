using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using YDock.Commands;
using YDock.Global;
using YDock.Interface;

namespace YDock.View
{
    public class DockMenu : CustomStyleMenu, IDisposable
    {
        public DockMenu(IDockItem targetObj)
        {
            _targetObj = targetObj;
            _InitMenuItem();
            ResourceExtension.LanaguageChanged += OnLanaguageChanged;
        }

        private IDockItem _targetObj;
        public IDockItem TargetObj { get { return _targetObj; } }

        private void _InitMenuItem()
        {
            MenuItem item = default(MenuItem);
            for (int i = 0; i < 5; i++)
            {
                item = new MenuItem();
                switch (i)
                {
                    case 0:
                        item.Header = Properties.Resources.Float;
                        item.Command = GlobalCommands.ToFloatCommand;
                        break;
                    case 1:
                        item.Header = Properties.Resources.Dock;
                        item.Command = GlobalCommands.ToDockCommand;
                        break;
                    case 2:
                        item.Header = Properties.Resources.Dock_Document;
                        item.Command = GlobalCommands.ToDockAsDocumentCommand;
                        break;
                    case 3:
                        item.Header = Properties.Resources.AutoHide;
                        item.Command = GlobalCommands.SwitchAutoHideStatusCommand;
                        break;
                    case 4:
                        item.Header = Properties.Resources.Hide;
                        item.Command = GlobalCommands.HideStatusCommand;
                        break;
                }
                Items.Add(item);
            }
        }

        private void OnLanaguageChanged(object sender, EventArgs e)
        {
            int index = 0;
            foreach (MenuItem item in Items)
            {
                switch (index)
                {
                    case 0:
                        item.Header = Properties.Resources.Float;
                        break;
                    case 1:
                        item.Header = Properties.Resources.Dock;
                        break;
                    case 2:
                        item.Header = Properties.Resources.Dock_Document;
                        break;
                    case 3:
                        item.Header = Properties.Resources.AutoHide;
                        break;
                    case 4:
                        item.Header = Properties.Resources.Hide;
                        break;
                }
                index++;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            CommandBindings.Add(new CommandBinding(GlobalCommands.ToFloatCommand, OnCommandExecute, OnCommandCanExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.ToDockCommand, OnCommandExecute, OnCommandCanExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.ToDockAsDocumentCommand, OnCommandExecute, OnCommandCanExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.SwitchAutoHideStatusCommand, OnCommandExecute, OnCommandCanExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.HideStatusCommand, OnCommandExecute, OnCommandCanExecute));
            base.OnInitialized(e);
        }

        private void OnCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_targetObj == null || _targetObj.IsDisposed)
            {
                e.CanExecute = false;
                return;
            }
            if (e.Command == GlobalCommands.ToFloatCommand)
                e.CanExecute = _targetObj.CanFloat;
            if (e.Command == GlobalCommands.ToDockCommand)
                e.CanExecute = _targetObj.CanDock;
            if (e.Command == GlobalCommands.ToDockAsDocumentCommand)
                e.CanExecute = _targetObj.CanDockAsDocument;
            if (e.Command == GlobalCommands.SwitchAutoHideStatusCommand)
                e.CanExecute = _targetObj.CanSwitchAutoHideStatus;
            if (e.Command == GlobalCommands.HideStatusCommand)
                e.CanExecute = _targetObj.CanHide;
        }

        private void OnCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (_targetObj == null || _targetObj.IsDisposed)
                return;
            if (e.Command == GlobalCommands.ToFloatCommand)
                _targetObj.ToFloat();
            if (e.Command == GlobalCommands.ToDockCommand)
                _targetObj.ToDock();
            if (e.Command == GlobalCommands.ToDockAsDocumentCommand)
                _targetObj.ToDockAsDocument();
            if (e.Command == GlobalCommands.SwitchAutoHideStatusCommand)
                _targetObj.SwitchAutoHideStatus();
            if (e.Command == GlobalCommands.HideStatusCommand)
                _targetObj.Hide();
        }

        public void Dispose()
        {
            ResourceExtension.LanaguageChanged -= OnLanaguageChanged;
            _targetObj = null;
        }
    }
}