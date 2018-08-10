using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace YDock.Commands
{
    public static class GlobalCommands
    {
        public static RoutedUICommand ToFloatCommand { get; set; }
        public static RoutedUICommand ToFloatAllCommand { get; set; }
        public static RoutedUICommand ToDockCommand { get; set; }
        public static RoutedUICommand ToDockAsDocumentCommand { get; set; }
        public static RoutedUICommand SwitchAutoHideStatusCommand { get; set; }
        public static RoutedUICommand HideStatusCommand { get; set; }
        public static RoutedUICommand CloseCommand { get; set; }
        public static RoutedUICommand CloseAllCommand { get; set; }
        public static RoutedUICommand CloseAllExceptCommand { get; set; }
        public static RoutedUICommand RestoreCommand { get; set; }
        public static RoutedUICommand MinimizeCommand { get; set; }
        public static RoutedUICommand MaximizeCommand { get; set; }
        static GlobalCommands()
        {
            ToFloatCommand = new RoutedUICommand();
            ToFloatAllCommand = new RoutedUICommand();
            ToDockCommand = new RoutedUICommand();
            ToDockAsDocumentCommand = new RoutedUICommand();
            SwitchAutoHideStatusCommand = new RoutedUICommand();
            HideStatusCommand = new RoutedUICommand();
            CloseCommand = new RoutedUICommand();
            CloseAllExceptCommand = new RoutedUICommand();
            CloseAllCommand = new RoutedUICommand();
            RestoreCommand = new RoutedUICommand();
            MinimizeCommand = new RoutedUICommand();
            MaximizeCommand = new RoutedUICommand();
        }
    }
}