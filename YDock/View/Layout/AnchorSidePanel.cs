using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using YDock.Model;

namespace YDock.View
{
    public class AnchorSidePanel : Panel
    {
        //此标志位表示Arrange时是否需要补偿子元素的宽度
        private bool _needCompensate = false;
        /// <summary>
        /// 测量阶段若子元素总宽度超过可用宽度，则对子元素的宽度进行排序后依次裁剪多余得到宽度
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var visibleChildren = InternalChildren.Cast<FrameworkElement>().Where(ele => ele.Visibility != Visibility.Collapsed).ToList();

            double height = 0.0;
            double width = 0.0;
            foreach (var child in visibleChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                width += child.DesiredSize.Width + child.Margin.Left + child.Margin.Right;
                height = Math.Max(height, child.DesiredSize.Height);
            }
            //超过可用宽度
            if (width > availableSize.Width)
            {
                _needCompensate = true;
                //超过的部分
                double exceed = width - availableSize.Width;
                //将元素按宽度从大到小排序,多余的长度从最长的元素开始裁剪
                visibleChildren.Sort(new ElementComparer<FrameworkElement>((a, b) => 
                {
                    if (a.DesiredSize.Width == b.DesiredSize.Width) return 0;
                    else if (a.DesiredSize.Width > b.DesiredSize.Width) return -1;
                    else return 1;
                }));

                //当前宽度为最大宽度的元素个数
                int currentCnt = 0;
                //当前最大宽度
                double currentWidth = visibleChildren[0].DesiredSize.Width;
                //若最大宽度有多个元素，则要一起进行裁剪
                foreach (var child in visibleChildren)
                {
                    if (child.DesiredSize.Width == currentWidth)
                        currentCnt++;
                    else break;
                }

                while(exceed > 0)
                {
                    //表示所有子元素裁剪后宽度全部一致了
                    if (currentCnt == visibleChildren.Count)
                    {
                        if (currentCnt * currentWidth >= exceed)
                            currentWidth -= exceed / currentCnt;
                        else currentWidth = 0;
                        exceed = 0;
                    }
                    else
                    {
                        //获得第二宽的元素的宽度作为本轮裁剪的目标值
                        double nextLen = visibleChildren[currentCnt].DesiredSize.Width;
                        if ((currentWidth - nextLen) * currentCnt >= exceed)
                        {
                            currentWidth -= exceed / currentCnt;
                            exceed = 0;
                        }
                        else
                        {
                            exceed -= (currentWidth - nextLen) * currentCnt;
                            for (int i = currentCnt; i < visibleChildren.Count; i++)
                                if (visibleChildren[currentCnt].DesiredSize.Width == nextLen)
                                    currentCnt++;
                                else break;
                            currentWidth = nextLen;
                        }
                    }
                }
                //裁剪后，对宽度改变的元素重新进行测量
                for (int i = 0; i < currentCnt; i++)
                    visibleChildren[i].Measure(new Size(currentWidth, height));
            }
            return new Size(Math.Min(width, availableSize.Width), height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var visibleChildren = InternalChildren.Cast<FrameworkElement>().Where(ele => ele.Visibility != Visibility.Collapsed);

            double wholeLength = visibleChildren.Sum(a => a.DesiredSize.Width);
            double delta = 0;

            //由于TextBlock的TextTrimming="CharacterEllipsis"时，由于字符裁剪会导致DesiredSize的Width变得比实际尺寸小，故在这里补偿
            if (wholeLength < finalSize.Width && _needCompensate)
            {
                _needCompensate = false;
                delta = (finalSize.Width - wholeLength) / visibleChildren.Count();
            }

            double offset = 0.0;
            foreach (FrameworkElement child in visibleChildren)
            {
                child.Arrange(new Rect(new Point(offset, 0), new Size(child.DesiredSize.Width + delta, finalSize.Height)));
                offset += child.DesiredSize.Width + delta;
            }

            return finalSize;
        }
    }
}