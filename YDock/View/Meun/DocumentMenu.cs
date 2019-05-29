using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YDock.Commands;
using YDock.Enum;
using YDock.Global;
using YDock.Interface;

namespace YDock.View
{
    public class DocumentMenu : CustomStyleMenu, IDisposable
    {
        public DocumentMenu(IDockElement targetObj)
        {
            _targetObj = targetObj;
            _InitMenuItem();
            ResourceExtension.LanaguageChanged += OnLanaguageChanged;
        }

        public IDockElement TargetObj { get { return _targetObj; } }
        private IDockElement _targetObj;

        private void _InitMenuItem()
        {
            MenuItem item = default(MenuItem);
            for (int i = 0; i < 5; i++)
            {
                item = new MenuItem() { Tag = i };
                switch (i)
                {
                    case 0:
                        item.Header = Properties.Resources._Close;
                        item.Command = GlobalCommands.CloseCommand;
                        break;
                    case 1:
                        item.Header = Properties.Resources.Close_All_Except;
                        item.Command = GlobalCommands.CloseAllExceptCommand;
                        break;
                    case 2:
                        item.Header = Properties.Resources.Close_All;
                        item.Command = GlobalCommands.CloseAllCommand;
                        break;
                    case 3:
                        Items.Add(new Separator());
                        item.Header = Properties.Resources.Float;
                        item.Command = GlobalCommands.ToFloatCommand;
                        break;
                    case 4:
                        item.Header = Properties.Resources.Float_All;
                        item.Command = GlobalCommands.ToFloatAllCommand;
                        break;
                }
                Items.Add(item);
            }
        }

        private void OnLanaguageChanged(object sender, EventArgs e)
        {
            foreach (var item in Items)
            {
                if (item is MenuItem)
                {
                    var _item = item as MenuItem;
                    switch ((int)_item.Tag)
                    {
                        case 0:
                            _item.Header = Properties.Resources._Close;
                            break;
                        case 1:
                            _item.Header = Properties.Resources.Close_All_Except;
                            break;
                        case 2:
                            _item.Header = Properties.Resources.Close_All;
                            break;
                        case 3:
                            _item.Header = Properties.Resources.Float;
                            break;
                        case 4:
                            _item.Header = Properties.Resources.Float_All;
                            break;
                    }
                }
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            CommandBindings.Add(new CommandBinding(GlobalCommands.CloseCommand, OnCommandExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.CloseAllExceptCommand, OnCommandExecute, OnCommandCanExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.CloseAllCommand, OnCommandExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.ToFloatCommand, OnCommandExecute, OnCommandCanExecute));
            CommandBindings.Add(new CommandBinding(GlobalCommands.ToFloatAllCommand, OnCommandExecute, OnCommandCanExecute));
            base.OnInitialized(e);
        }

        private void OnCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_targetObj == null || _targetObj.IsDisposed)
            {
                e.CanExecute = false;
                return;
            }
            if (e.Command == GlobalCommands.CloseAllExceptCommand)
                e.CanExecute = _targetObj.Container.Children.Count() > 1;
            if (e.Command == GlobalCommands.ToFloatCommand)
                e.CanExecute = _targetObj.CanFloat;
            if (e.Command == GlobalCommands.ToFloatAllCommand)
                e.CanExecute = _targetObj.Container.Mode != DockMode.Float;
        }

        private void OnCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (_targetObj == null || _targetObj.IsDisposed)
                return;
            if (e.Command == GlobalCommands.CloseCommand)
                _targetObj.Hide();
            if (e.Command == GlobalCommands.CloseAllExceptCommand)
                _targetObj.Container.CloseAllExcept(_targetObj);
            if (e.Command == GlobalCommands.CloseAllCommand)
                _targetObj.Container.CloseAll();
            if (e.Command == GlobalCommands.ToFloatCommand)
                _targetObj.ToFloat();
            if (e.Command == GlobalCommands.ToFloatAllCommand)
                _targetObj.Container.ToFloat();
        }

        public void Dispose()
        {
            _targetObj = null;
            ResourceExtension.LanaguageChanged -= OnLanaguageChanged;
        }
    }
}