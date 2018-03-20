using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            _thicknessAnimation = new ThicknessAnimation(new Thickness(1), new Duration(TimeSpan.FromMilliseconds(1)))
            {
                BeginTime = TimeSpan.FromSeconds(0.4)
            };
            _backgroundAnimation = new ColorAnimation(Colors.Transparent, ResourceManager.SplitterBrushVertical.Color, new Duration(TimeSpan.FromMilliseconds(1)));
            _borderBrushAnimation = new ColorAnimation(Colors.Transparent, ResourceManager.SplitterBrushHorizontal.Color, new Duration(TimeSpan.FromMilliseconds(1)))
            {
                BeginTime = TimeSpan.FromSeconds(0.2)
            };
            _board = new Storyboard();
            _board.Children.Add(_thicknessAnimation);
            _board.Children.Add(_backgroundAnimation);
            _board.Children.Add(_borderBrushAnimation);
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


        private Timeline _thicknessAnimation;
        private Timeline _backgroundAnimation;
        private Timeline _borderBrushAnimation;
        private Storyboard _board;
        public override void Recreate()
        {
            if (Child == null) return;
            if (_needReCreate)
            {
                NeedReCreate = false;
                var layoutCtrl = Child as BaseGroupControl;
                layoutCtrl.IsDraggingFromDock = false;
                header.Visibility = Visibility.Visible;
                Storyboard.SetTarget(_thicknessAnimation, layoutCtrl);
                Storyboard.SetTargetProperty(_thicknessAnimation, new PropertyPath(BorderThicknessProperty));
                Storyboard.SetTarget(_backgroundAnimation, this);
                Storyboard.SetTargetProperty(_backgroundAnimation, new PropertyPath("(0).(1)",new DependencyProperty[] { BackgroundProperty, SolidColorBrush.ColorProperty }));
                Storyboard.SetTarget(_borderBrushAnimation, this);
                Storyboard.SetTargetProperty(_borderBrushAnimation, new PropertyPath("(0).(1)", new DependencyProperty[] { BorderBrushProperty, SolidColorBrush.ColorProperty }));
                _board.Begin(this);
            }
            else
            {
                NeedReCreate = true;
                header.Visibility = Visibility.Hidden;
                Background = Brushes.Transparent;
                BorderBrush = Brushes.Transparent;
                var layoutCtrl = Child as BaseGroupControl;
                layoutCtrl.BorderThickness = new Thickness(1);
                layoutCtrl.IsDraggingFromDock = true;
            }
        }

        DockPanel header;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            header = (DockPanel)GetTemplateChild("PART_Header");
            if (_needReCreate)
                header.Visibility = Visibility.Hidden;
        }
    }
}