using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
    }
}