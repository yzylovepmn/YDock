using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using YDock.Interface;

namespace YDock.View
{
    public class DocumentGroupWindow : BaseFloatWindow
    {
        static DocumentGroupWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentGroupWindow), new FrameworkPropertyMetadata(typeof(DocumentGroupWindow)));
        }

        public override void AttachChild(IDockView child, int index)
        {
            //后面的2是border Thickness
            _widthEceeed += Constants.DocumentWindowPadding * 2 + 2;
            _heightEceeed += Constants.DocumentWindowPadding * 2 + Constants.FloatWindowHeaderHeight + 2;
            base.AttachChild(child, index);
        }

        public override void Recreate()
        {
            base.Recreate();
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