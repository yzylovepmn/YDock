using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace YDock.View
{
    public class DocumentGroupWindow : BaseFloatWindow
    {
        static DocumentGroupWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentGroupWindow), new FrameworkPropertyMetadata(typeof(DocumentGroupWindow)));
        }
    }
}