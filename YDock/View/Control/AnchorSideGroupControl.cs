﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class AnchorSideGroupControl : BaseGroupControl
    {
        static AnchorSideGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorSideGroupControl), new FrameworkPropertyMetadata(typeof(AnchorSideGroupControl)));
            FocusableProperty.OverrideMetadata(typeof(AnchorSideGroupControl), new FrameworkPropertyMetadata(false));
        }

        internal AnchorSideGroupControl(ILayoutGroup model) : base(model)
        {
            
        }

        public override DragMode Mode
        {
            get
            {
                return DragMode.Anchor;
            }
        }

        public override void OnDrop(DragItem source, int flag)
        {
            
        }

        public override void CreateDropWindow()
        {
            
        }
    }
}