using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    public class DockSideItemControl : ContentControl
    {
        static DockSideItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockSideItemControl), new FrameworkPropertyMetadata(typeof(DockSideItemControl)));
            FocusableProperty.OverrideMetadata(typeof(DockSideItemControl), new FrameworkPropertyMetadata(false));
        }

        public DockSideItemControl(ILayoutGroup container)
        {
            _container = container;
        }

        private ILayoutGroup _container;
        public ILayoutGroup Container
        {
            get
            {
                return _container;
            }

            set
            {
                if (_container != value)
                    _container = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    var parent = _container as DockSideGroupControl;
        //    var dock = parent.Model.DockManager;
        //    AnchorSidePanel asp = default(AnchorSidePanel);
        //    switch (parent.Model.Side)
        //    {
        //        case DockSide.Left:
        //            asp = dock.LayoutRootPanel.LeftSideContent;
        //            break;
        //        case DockSide.Right:
        //            asp = dock.LayoutRootPanel.RightSideContent;
        //            break;
        //        case DockSide.Top:
        //            asp = dock.LayoutRootPanel.TopSideContent;
        //            break;
        //        case DockSide.Bottom:
        //            asp = dock.LayoutRootPanel.BottomSideContent;
        //            break;
        //    }

        //    if (asp != null)
        //    {
        //        LayoutElement ele = Content as LayoutElement;
        //        if (!ele.IsSplitMode)
        //        {
        //            if (asp.NormalDocument.Model == ele)
        //                asp.NormalDocument.Model = null;
        //            else
        //            {
        //                if (!asp.HasContent || (!asp.IsFullMode && asp.NormalDocument.Model != null))
        //                {
        //                    switch (parent.Model.Side)
        //                    {
        //                        case DockSide.Left:
        //                        case DockSide.Right:
        //                            asp.ContentSideLenght = Math.Max(ele.Width, Constants.SideLength);
        //                            if (asp.NormalDocument.Model != null)
        //                                ele.Height = asp.NormalDocument.Model.Height;
        //                            break;
        //                        case DockSide.Top:
        //                        case DockSide.Bottom:
        //                            asp.ContentSideLenght = Math.Max(ele.Height, Constants.SideLength);
        //                            if (asp.NormalDocument.Model != null)
        //                                ele.Width = asp.NormalDocument.Model.Width;
        //                            break;
        //                    }
        //                }
        //                asp.NormalDocument.Model = ele;
        //            }
        //        }
        //        else
        //        {
        //            if (asp.SplitDocument.Model == ele)
        //                asp.SplitDocument.Model = null;
        //            else
        //            {
        //                if (!asp.HasContent || (!asp.IsFullMode && asp.SplitDocument.Model != null))
        //                {
        //                    switch (parent.Model.Side)
        //                    {
        //                        case DockSide.Left:
        //                        case DockSide.Right:
        //                            asp.ContentSideLenght = Math.Max(ele.Width, Constants.SideLength);
        //                            if (asp.SplitDocument.Model != null)
        //                                ele.Height = asp.SplitDocument.Model.Height;
        //                            break;
        //                        case DockSide.Top:
        //                        case DockSide.Bottom:
        //                            asp.ContentSideLenght = Math.Max(ele.Height, Constants.SideLength);
        //                            if (asp.SplitDocument.Model != null)
        //                                ele.Width = asp.SplitDocument.Model.Width;
        //                            break;
        //                    }
        //                }
        //                asp.SplitDocument.Model = ele;
        //            }
        //        }
        //        if (!asp.HasContent)
        //            asp.ContentSideLenght = 0;
        //        if (asp.IsFullMode)
        //        {
        //            switch (parent.Model.Side)
        //            {
        //                case DockSide.Left:
        //                case DockSide.Right:
        //                    ele.Width = asp.ContentSideLenght;
        //                    if (ele.IsSplitMode)
        //                        ele.Height = asp.NormalDocument.Model.Height;
        //                    else ele.Height = asp.SplitDocument.Model.Height;
        //                    break;
        //                case DockSide.Top:
        //                case DockSide.Bottom:
        //                    ele.Height = asp.ContentSideLenght;
        //                    if (ele.IsSplitMode)
        //                        ele.Width = asp.NormalDocument.Model.Width;
        //                    else ele.Width = asp.SplitDocument.Model.Width;
        //                    break;
        //            }
        //        }
        //    }

        //    base.OnMouseLeftButtonUp(e);
        //}
    }
}