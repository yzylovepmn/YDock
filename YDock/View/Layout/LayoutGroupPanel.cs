using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using YDock.Enum;
using YDock.Interface;
using YDock.Model;

namespace YDock.View
{
    /// <summary>
    /// the core class for layout and resize region
    /// </summary>
    public class LayoutGroupPanel : Panel, ILayoutPanel, IDragTarget
    {
        public LayoutGroupPanel(DockSide side = DockSide.None)
        {
            _side = side;
        }

        /// <summary>
        /// 表示该Panel的Children中除了<see cref="LayoutDragSplitter"/>就是<see cref="LayoutDocumentGroupControl"/>
        /// </summary>
        public bool IsDocumentPanel
        {
            get
            {
                return this is LayoutGroupDocumentPanel;
            }
        }


        /// <summary>
        /// 表示该Panel的Children中除了<see cref="LayoutDragSplitter"/>就是<see cref="AnchorSideGroupControl"/>
        /// </summary>
        public bool IsAnchorPanel
        {
            get
            {
                return _isAnchorPanel;
            }
            internal set
            {
                if (_isAnchorPanel != value)
                    _isAnchorPanel = value;
            }
        }
        private bool _isAnchorPanel = false;

        /// <summary>
        /// 表示该Panel的Children中递归包含<see cref="LayoutDocumentGroupControl"/>
        /// </summary>
        public bool ContainDocument
        {
            get
            {
                return _side == DockSide.None;
            }
        }

        public bool IsEmpty
        {
            get { return Children.Count == 0; }
        }

        public int Count
        {
            get { return Children.Count; }
        }

        public bool IsRootPanel
        {
            get
            {
                return DockViewParent is LayoutRootPanel;
            }
        }

        public DockManager DockManager
        {
            get
            {
                var parent = DockViewParent;
                while (parent?.DockViewParent != null)
                {
                    if (parent.DockViewParent is DockManager)
                        return parent.DockViewParent as DockManager;
                    else parent = parent.DockViewParent;
                }
                return null;
            }
        }

        private double _desiredWidth;
        public double DesiredWidth
        {
            get
            {
                return _desiredWidth;
            }
            set
            {
                if (_desiredWidth != value)
                    _desiredWidth = value;
            }
        }

        private double _desiredHeight;
        public double DesiredHeight
        {
            get
            {
                return _desiredHeight;
            }
            set
            {
                if (_desiredHeight != value)
                    _desiredHeight = value;
            }
        }

        private Direction _direction = Direction.None;
        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        private DockSide _side;
        public DockSide Side
        {
            get
            {
                return _side;
            }
            internal set
            {
                if (_side != value)
                {
                    _side = value;
                    foreach (var child in Children)
                    {
                        if (child is ILayoutPanel)
                            (child as LayoutGroupPanel).Side = value;
                        if (child is BaseGroupControl)
                            ((child as BaseGroupControl).Model as BaseLayoutGroup).Side = value;
                    }
                }
            }
        }

        public IDockModel Model
        {
            get
            {
                return null;
            }
        }

        public IDockView DockViewParent
        {
            get
            {
                return Parent == null ? null : Parent as IDockView;
            }
        }

        public DragMode Mode
        {
            get
            {
                return DragMode.RootPanel;
            }
        }

        #region MeasureOverride
        protected override Size MeasureOverride(Size availableSize)
        {
            if (InternalChildren.Count == 0) return availableSize;
            if (IsAnchorPanel || IsDocumentPanel)
                return _MeasureOverrideFull(availableSize);
            else return _MeasureOverrideSplit(availableSize);
        }

        /// <summary>
        /// Children全部为同一种<see cref="ILayoutGroupControl"/>时调用
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        private Size _MeasureOverrideFull(Size availableSize)
        {
            var layoutgroups = new List<ILayoutSize>();
            for (int i = 0; i < InternalChildren.Count; i += 2)
                layoutgroups.Add(InternalChildren[i] as ILayoutSize);

            double wholelength = 0;
            double availableLength = 0;
            var stars = new List<double>();
            switch (Direction)
            {
                case Direction.LeftToRight:
                    wholelength = layoutgroups.Sum(group => group.DesiredWidth);
                    foreach (var group in layoutgroups)
                        stars.Add(group.DesiredWidth / wholelength);
                    availableLength = Math.Max(availableSize.Width - Constants.SplitterSpan * (layoutgroups.Count - 1), 0);
                    //当children中的最小可用空间都大于SideLength，则按每个child的实际长度比来分配空间
                    if (availableLength * stars.Min() >= Constants.SideLength)
                    {
                        for (int i = 0; i < InternalChildren.Count; i += 2)
                        {
                            (InternalChildren[i] as ILayoutSize).DesiredWidth = stars[i / 2] * availableLength;
                            InternalChildren[i].Measure(new Size((InternalChildren[i] as ILayoutSize).DesiredWidth, availableSize.Height));
                            if (i + 1 < InternalChildren.Count)
                                InternalChildren[i + 1].Measure(new Size(Constants.SplitterSpan, availableSize.Height));
                        }
                    }
                    else
                    {
                        double deceed = wholelength - availableLength;
                        if (deceed >= 0 && (availableLength - layoutgroups.Count * Constants.SideLength > 0))
                        {
                            for (int i = 0; i < InternalChildren.Count; i += 2)
                            {
                                double childlen = layoutgroups[i / 2].DesiredWidth;
                                if (deceed > 0)
                                {
                                    if (childlen - deceed > Constants.SideLength)
                                    {
                                        InternalChildren[i].Measure(new Size(childlen - deceed, availableSize.Height));
                                        deceed = 0;
                                    }
                                    else
                                    {
                                        InternalChildren[i].Measure(new Size(Constants.SideLength, availableSize.Height));
                                        deceed -= childlen - Constants.SideLength;
                                    }
                                }
                                else InternalChildren[i].Measure(new Size(childlen, availableSize.Height));
                                if (i + 1 < InternalChildren.Count)
                                    InternalChildren[i + 1].Measure(new Size(Constants.SplitterSpan, availableSize.Height));
                            }
                        }
                        else//表示可用空间小于最小需求空间大小，故执行剪裁
                        {
                            return _ClipToBounds_Measure(availableSize);
                        }
                    }
                    break;
                case Direction.UpToDown:
                    wholelength = layoutgroups.Sum(group => group.DesiredHeight);
                    foreach (var group in layoutgroups)
                        stars.Add(group.DesiredHeight / wholelength);
                    availableLength = Math.Max(availableSize.Height - Constants.SplitterSpan * (layoutgroups.Count - 1), 0);
                    //当children中的最小可用空间都大于SideLength，则按每个child的实际长度比来分配空间
                    if (availableLength * stars.Min() >= Constants.SideLength)
                    {
                        for (int i = 0; i < InternalChildren.Count; i += 2)
                        {
                            (InternalChildren[i] as ILayoutSize).DesiredHeight = stars[i / 2] * availableLength;
                            InternalChildren[i].Measure(new Size(availableSize.Width, (InternalChildren[i] as ILayoutSize).DesiredHeight));
                            if (i + 1 < InternalChildren.Count)
                                InternalChildren[i + 1].Measure(new Size(availableSize.Width, Constants.SplitterSpan));
                        }
                    }
                    else
                    {
                        double deceed = wholelength - availableLength;
                        if (deceed >= 0 && (availableLength - layoutgroups.Count * Constants.SideLength > 0))
                        {
                            for (int i = 0; i < InternalChildren.Count; i += 2)
                            {
                                double childlen = layoutgroups[i / 2].DesiredHeight;
                                if (deceed > 0)
                                {
                                    if (childlen - deceed > Constants.SideLength)
                                    {
                                        InternalChildren[i].Measure(new Size(availableSize.Width, childlen - deceed));
                                        deceed = 0;
                                    }
                                    else
                                    {
                                        InternalChildren[i].Measure(new Size(availableSize.Width, Constants.SideLength));
                                        deceed -= childlen - Constants.SideLength;
                                    }
                                }
                                else InternalChildren[i].Measure(new Size(availableSize.Width, childlen));
                                if (i + 1 < InternalChildren.Count)
                                    InternalChildren[i + 1].Measure(new Size(availableSize.Width, Constants.SplitterSpan));
                            }
                        }
                        else//表示可用空间小于最小需求空间大小，故执行剪裁
                        {
                            return _ClipToBounds_Measure(availableSize);
                        }
                    }
                    break;
                case Direction.None:
                    InternalChildren[0].Measure(availableSize);
                    break;
            }

            return availableSize;
        }

        private Size _MeasureOverrideSplit(Size availableSize)
        {
            ILayoutSize sizechild = default(ILayoutSize);
            FrameworkElement documentChild = default(FrameworkElement);
            //这里的wholeLength表示不需要调整child类型为LayoutDocumentGroupControl大小的最小总长度
            double wholeLength = 0;
            switch (Direction)
            {
                case Direction.LeftToRight:
                    foreach (var child in InternalChildren)
                    {
                        if (child is LayoutDragSplitter)
                            wholeLength += Constants.SplitterSpan;
                        else
                        {
                            if (_IsDocumentChild(child))
                                wholeLength += _GetMinLength(child);
                            else wholeLength += (child as ILayoutSize).DesiredWidth;
                        }
                    }
                    //自动调整child类型为LayoutDocumentGroupControl的大小，以适应布局
                    if (wholeLength <= availableSize.Width)
                    {
                        double useLength = 0;
                        foreach (FrameworkElement child in InternalChildren)
                        {
                            if (child is LayoutDragSplitter)
                            {
                                useLength += Constants.SplitterSpan;
                                child.Measure(new Size(Constants.SplitterSpan, availableSize.Height));
                            }
                            else
                            {
                                if (documentChild == null && _IsDocumentChild(child))
                                    documentChild = child;
                                else
                                {
                                    useLength += (child as ILayoutSize).DesiredWidth;
                                    child.Measure(new Size((child as ILayoutSize).DesiredWidth, availableSize.Height));
                                }
                            }
                        }
                        (documentChild as ILayoutSize).DesiredWidth = availableSize.Width - useLength;
                        (documentChild as ILayoutSize).DesiredHeight = availableSize.Height;
                        documentChild.Measure(new Size((documentChild as ILayoutSize).DesiredWidth, (documentChild as ILayoutSize).DesiredHeight));
                    }
                    else//否则减小其它child的大小，以适应布局
                    {
                        //这里计算所有child的最小长度和
                        if (_GetMinLength(this) <= availableSize.Width)
                        {
                            double exceed = wholeLength - availableSize.Width;
                            foreach (FrameworkElement child in InternalChildren)
                            {
                                if (_IsDocumentChild(child))
                                    child.Measure(new Size(_GetMinLength(child), availableSize.Height));
                                else
                                {
                                    if (child is LayoutDragSplitter)
                                        child.Measure(new Size(Constants.SplitterSpan, availableSize.Height));
                                    else
                                    {
                                        sizechild = child as ILayoutSize;
                                        if (exceed > 0)
                                        {
                                            if (sizechild.DesiredWidth - exceed >= Constants.SideLength)
                                            {
                                                child.Measure(new Size(sizechild.DesiredWidth - exceed, availableSize.Height));
                                                exceed = 0;
                                            }
                                            else
                                            {
                                                exceed -= sizechild.DesiredWidth - Constants.SideLength;
                                                child.Measure(new Size(Constants.SideLength, availableSize.Height));
                                            }
                                        }
                                        else child.Measure(new Size(sizechild.DesiredWidth, availableSize.Height));
                                    }
                                }
                            }
                        }
                        else//表示可用空间小于最小需求空间大小，故执行剪裁
                        {
                            return _ClipToBounds_Measure(availableSize);
                        }
                    }
                    break;
                case Direction.UpToDown:
                    foreach (var child in InternalChildren)
                    {
                        if (child is LayoutDragSplitter)
                            wholeLength += Constants.SplitterSpan;
                        else
                        {
                            if (_IsDocumentChild(child))
                                wholeLength += _GetMinLength(child);
                            else wholeLength += (child as ILayoutSize).DesiredHeight;
                        }
                    }
                    //自动调整child类型为LayoutDocumentGroupControl的大小，以适应布局
                    if (wholeLength <= availableSize.Height)
                    {
                        double useLength = 0;
                        foreach (FrameworkElement child in InternalChildren)
                        {
                            if (child is LayoutDragSplitter)
                            {
                                useLength += Constants.SplitterSpan;
                                child.Measure(new Size(availableSize.Width, Constants.SplitterSpan));
                            }
                            else
                            {
                                if (documentChild == null && _IsDocumentChild(child))
                                    documentChild = child;
                                else
                                {
                                    useLength += (child as ILayoutSize).DesiredHeight;
                                    child.Measure(new Size(availableSize.Width, (child as ILayoutSize).DesiredHeight));
                                }
                            }
                        }
                        (documentChild as ILayoutSize).DesiredWidth = availableSize.Width;
                        (documentChild as ILayoutSize).DesiredHeight = availableSize.Height - useLength;
                        documentChild.Measure(new Size(availableSize.Width, availableSize.Height - useLength));
                    }
                    else//否则减小其它child的大小，以适应布局
                    {
                        //这里计算所有child的最小长度和
                        if (_GetMinLength(this) <= availableSize.Height)
                        {
                            double exceed = wholeLength - availableSize.Height;
                            foreach (FrameworkElement child in InternalChildren)
                            {
                                if (_IsDocumentChild(child))
                                    child.Measure(new Size(availableSize.Width, _GetMinLength(child)));
                                else
                                {
                                    if (child is LayoutDragSplitter)
                                        child.Measure(new Size(availableSize.Width, Constants.SplitterSpan));
                                    else
                                    {
                                        sizechild = child as ILayoutSize;
                                        if (exceed > 0)
                                        {
                                            if (sizechild.DesiredHeight - exceed >= Constants.SideLength)
                                            {
                                                child.Measure(new Size(availableSize.Width, sizechild.DesiredHeight - exceed));
                                                exceed = 0;
                                            }
                                            else
                                            {
                                                exceed -= sizechild.DesiredHeight - Constants.SideLength;
                                                child.Measure(new Size(availableSize.Width, Constants.SideLength));
                                            }
                                        }
                                        else child.Measure(new Size(availableSize.Width, sizechild.DesiredHeight));
                                    }
                                }
                            }
                        }
                        else//表示可用空间小于最小需求空间大小，故执行剪裁
                        {
                            return _ClipToBounds_Measure(availableSize);
                        }
                    }
                    break;
            }
            return availableSize;
        }

        private Size _ClipToBounds_Measure(Size availableSize)
        {
            double minLength;
            switch (Direction)
            {
                case Direction.LeftToRight:
                    double availableLength = availableSize.Width;
                    for (int i = 0; i < InternalChildren.Count; i += 2)
                    {
                        minLength = _GetMinLength(InternalChildren[i]);
                        if (availableLength >= minLength)
                        {
                            InternalChildren[i].Measure(new Size(minLength, availableSize.Height));
                            availableLength -= minLength;
                        }
                        else
                        {
                            InternalChildren[i].Measure(new Size(availableLength, availableSize.Height));
                            availableLength = 0;
                        }
                        if (i + 1 < InternalChildren.Count)
                        {
                            if (availableLength >= Constants.SplitterSpan)
                            {
                                InternalChildren[i + 1].Measure(new Size(Constants.SplitterSpan, availableSize.Height));
                                availableLength -= Constants.SplitterSpan;
                            }
                            else
                            {
                                InternalChildren[i + 1].Measure(new Size(availableLength, availableSize.Height));
                                availableLength = 0;
                            }
                        }
                    }
                    break;
                case Direction.UpToDown:
                    availableLength = availableSize.Height;
                    for (int i = 0; i < InternalChildren.Count; i += 2)
                    {
                        minLength = _GetMinLength(InternalChildren[i]);
                        if (availableLength >= minLength)
                        {
                            InternalChildren[i].Measure(new Size(availableSize.Width, minLength));
                            availableLength -= minLength;
                        }
                        else
                        {
                            InternalChildren[i].Measure(new Size(availableSize.Width, availableLength));
                            availableLength = 0;
                        }
                        if (i + 1 < InternalChildren.Count)
                        {
                            if (availableLength >= Constants.SplitterSpan)
                            {
                                InternalChildren[i + 1].Measure(new Size(availableSize.Width, Constants.SplitterSpan));
                                availableLength -= Constants.SplitterSpan;
                            }
                            else
                            {
                                InternalChildren[i + 1].Measure(new Size(availableSize.Width, availableLength));
                                availableLength = 0;
                            }
                        }
                    }
                    break;
            }
            return availableSize;
        }
        #endregion

        #region ArrangeOverride
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0) return finalSize;
            if (IsAnchorPanel || IsDocumentPanel)
                return _ArrangeOverrideFull(finalSize);
            else return _ArrangeOverrideSplit(finalSize);
        }

        private Size _ArrangeOverrideFull(Size finalSize)
        {
            var layoutgroups = new List<ILayoutSize>();
            for (int i = 0; i < InternalChildren.Count; i += 2)
                layoutgroups.Add(InternalChildren[i] as ILayoutSize);

            double wholelength = 0;
            double availableLength = 0;
            var stars = new List<double>();
            switch (Direction)
            {
                case Direction.LeftToRight:
                    wholelength = layoutgroups.Sum(group => group.DesiredWidth);
                    foreach (var group in layoutgroups)
                        stars.Add(group.DesiredWidth / wholelength);
                    availableLength = Math.Max(finalSize.Width - Constants.SplitterSpan * (layoutgroups.Count - 1), 0);
                    //当children中的最小可用空间都大于SideLength，则按每个child的实际长度比来分配空间
                    if (availableLength * stars.Min() >= Constants.SideLength)
                        return _ArrangeUniverse(finalSize);
                    else
                    {
                        double deceed = wholelength - availableLength;
                        if (deceed >= 0 && (availableLength - layoutgroups.Count * Constants.SideLength > 0))
                        {
                            double offset = 0;
                            for (int i = 0; i < InternalChildren.Count; i += 2)
                            {
                                double childlen = layoutgroups[i / 2].DesiredWidth;
                                if (deceed > 0)
                                {
                                    if (childlen - deceed > Constants.SideLength)
                                    {
                                        InternalChildren[i].Arrange(new Rect(new Point(offset, 0), new Size(childlen - deceed, finalSize.Height)));
                                        offset += childlen - deceed;
                                        deceed = 0;
                                    }
                                    else
                                    {
                                        InternalChildren[i].Arrange(new Rect(new Point(offset, 0), new Size(Constants.SideLength, finalSize.Height)));
                                        offset += Constants.SideLength;
                                        deceed -= childlen - Constants.SideLength;
                                    }
                                }
                                else
                                {
                                    InternalChildren[i].Arrange(new Rect(new Point(offset, 0), new Size(childlen, finalSize.Height)));
                                    offset += childlen;
                                }
                                if (i + 1 < InternalChildren.Count)
                                {
                                    InternalChildren[i + 1].Arrange(new Rect(new Point(offset, 0), new Size(Constants.SplitterSpan, finalSize.Height)));
                                    offset += Constants.SplitterSpan;
                                }
                            }
                        }
                        else//表示可用空间小于最小需求空间大小，故执行剪裁
                        {
                            return _ClipToBounds_Arrange(finalSize);
                        }
                    }
                    break;
                case Direction.UpToDown:
                    wholelength = layoutgroups.Sum(group => group.DesiredHeight);
                    foreach (var group in layoutgroups)
                        stars.Add(group.DesiredHeight / wholelength);
                    availableLength = Math.Max(finalSize.Height - Constants.SplitterSpan * (layoutgroups.Count - 1), 0);
                    //当children中的最小可用空间都大于SideLength，则按每个child的实际长度比来分配空间
                    if (availableLength * stars.Min() >= Constants.SideLength)
                        return _ArrangeUniverse(finalSize);
                    else
                    {
                        double deceed = wholelength - availableLength;
                        if (deceed >= 0 && (availableLength - layoutgroups.Count * Constants.SideLength > 0))
                        {
                            double offset = 0;
                            for (int i = 0; i < InternalChildren.Count; i += 2)
                            {
                                double childlen = layoutgroups[i / 2].DesiredHeight;
                                if (deceed > 0)
                                {
                                    if (childlen - deceed > Constants.SideLength)
                                    {
                                        InternalChildren[i].Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, childlen - deceed)));
                                        offset += childlen - deceed;
                                        deceed = 0;
                                    }
                                    else
                                    {
                                        InternalChildren[i].Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, Constants.SideLength)));
                                        offset += Constants.SideLength;
                                        deceed -= childlen - Constants.SideLength;
                                    }
                                }
                                else
                                {
                                    InternalChildren[i].Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, childlen)));
                                    offset += childlen;
                                }
                                if (i + 1 < InternalChildren.Count)
                                {
                                    InternalChildren[i + 1].Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, Constants.SplitterSpan)));
                                    offset += Constants.SplitterSpan;
                                }
                            }
                        }
                        else//表示可用空间小于最小需求空间大小，故执行剪裁
                        {
                            return _ClipToBounds_Arrange(finalSize);
                        }
                    }
                    break;
                case Direction.None:
                    InternalChildren[0].Arrange(new Rect(new Point(), finalSize));
                    break;
            }

            return finalSize;
        }

        private Size _ArrangeOverrideSplit(Size finalSize)
        {
            ILayoutSize sizechild = default(ILayoutSize);
            //这里的wholeLength表示不需要调整child类型为LayoutDocumentGroupControl大小的最小总长度
            double wholeLength = 0;
            switch (Direction)
            {
                case Direction.LeftToRight:
                    foreach (var child in InternalChildren)
                    {
                        if (child is LayoutDragSplitter)
                            wholeLength += Constants.SplitterSpan;
                        else
                        {
                            if (_IsDocumentChild(child))
                                wholeLength += _GetMinLength(child);
                            else wholeLength += (child as ILayoutSize).DesiredWidth;
                        }
                    }
                    //自动调整child类型为LayoutDocumentGroupControl的大小，以适应布局
                    if (wholeLength <= finalSize.Width)
                        return _ArrangeUniverse(finalSize);
                    else//否则减小其它child的大小，以适应布局
                    {
                        //这里计算所有child的最小长度和
                        if (_GetMinLength(this) <= finalSize.Width)
                        {
                            double offset = 0;
                            double exceed = wholeLength - finalSize.Width;
                            foreach (FrameworkElement child in InternalChildren)
                            {
                                if (_IsDocumentChild(child))
                                {
                                    child.Arrange(new Rect(new Point(offset, 0), new Size(_GetMinLength(child), finalSize.Height)));
                                    offset += _GetMinLength(child);
                                }
                                else
                                {
                                    if (child is LayoutDragSplitter)
                                    {
                                        child.Arrange(new Rect(new Point(offset, 0), new Size(Constants.SplitterSpan, finalSize.Height)));
                                        offset += Constants.SplitterSpan;
                                    }
                                    else
                                    {
                                        sizechild = child as ILayoutSize;
                                        if (exceed > 0)
                                        {
                                            if (sizechild.DesiredWidth - exceed >= Constants.SideLength)
                                            {
                                                child.Arrange(new Rect(new Point(offset, 0), new Size(sizechild.DesiredWidth - exceed, finalSize.Height)));
                                                offset += sizechild.DesiredWidth - exceed;
                                                exceed = 0;
                                            }
                                            else
                                            {
                                                exceed -= sizechild.DesiredWidth - Constants.SideLength;
                                                child.Arrange(new Rect(new Point(offset, 0), new Size(Constants.SideLength, finalSize.Height)));
                                                offset += Constants.SideLength;
                                            }
                                        }
                                        else
                                        {
                                            child.Arrange(new Rect(new Point(offset, 0), new Size(sizechild.DesiredWidth, finalSize.Height)));
                                            offset += sizechild.DesiredWidth;
                                        }
                                    }
                                }
                            }
                        }
                        else//表示可用空间小于最小需求空间大小，故执行剪裁
                        {
                            _ClipToBounds_Arrange(finalSize);
                        }
                    }
                    break;
                case Direction.UpToDown:
                    foreach (var child in InternalChildren)
                    {
                        if (child is LayoutDragSplitter)
                            wholeLength += Constants.SplitterSpan;
                        else
                        {
                            if (_IsDocumentChild(child))
                                wholeLength += _GetMinLength(child);
                            else wholeLength += (child as ILayoutSize).DesiredHeight;
                        }
                    }
                    //自动调整child类型为LayoutDocumentGroupControl的大小，以适应布局
                    if (wholeLength <= finalSize.Height)
                        return _ArrangeUniverse(finalSize);
                    else//否则减小其它child的大小，以适应布局
                    {
                        //这里计算所有child的最小长度和
                        if (_GetMinLength(this) <= finalSize.Height)
                        {
                            double offset = 0;
                            double exceed = wholeLength - finalSize.Height;
                            foreach (FrameworkElement child in InternalChildren)
                            {
                                if (_IsDocumentChild(child))
                                {
                                    child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, _GetMinLength(child))));
                                    offset += _GetMinLength(child);
                                }
                                else
                                {
                                    if (child is LayoutDragSplitter)
                                    {
                                        child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, Constants.SplitterSpan)));
                                        offset += Constants.SplitterSpan;
                                    }
                                    else
                                    {
                                        sizechild = child as ILayoutSize;
                                        if (exceed > 0)
                                        {
                                            if (sizechild.DesiredHeight - exceed >= Constants.SideLength)
                                            {
                                                child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, sizechild.DesiredHeight - exceed)));
                                                offset += sizechild.DesiredHeight - exceed;
                                                exceed = 0;
                                            }
                                            else
                                            {
                                                exceed -= sizechild.DesiredHeight - Constants.SideLength;
                                                child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, Constants.SideLength)));
                                                offset += Constants.SideLength;
                                            }
                                        }
                                        else
                                        {
                                            child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, sizechild.DesiredHeight)));
                                            offset += sizechild.DesiredHeight;
                                        }
                                    }
                                }
                            }
                        }
                        else//表示可用空间小于最小需求空间大小，故执行剪裁
                        {
                            _ClipToBounds_Arrange(finalSize);
                        }
                    }
                    break;
            }

            return finalSize;
        }

        private Size _ArrangeUniverse(Size finalSize)
        {
            double offset = 0;
            switch (Direction)
            {
                case Direction.LeftToRight:
                    foreach (FrameworkElement child in InternalChildren)
                    {
                        if (child is LayoutDragSplitter)
                        {
                            child.Arrange(new Rect(new Point(offset, 0), new Size(Constants.SplitterSpan, finalSize.Height)));
                            offset += Constants.SplitterSpan;
                        }
                        else
                        {
                            child.Arrange(new Rect(new Point(offset, 0), new Size((child as ILayoutSize).DesiredWidth, finalSize.Height)));
                            offset += (child as ILayoutSize).DesiredWidth;
                        }
                    }
                    break;
                case Direction.UpToDown:
                    foreach (FrameworkElement child in InternalChildren)
                    {
                        if (child is LayoutDragSplitter)
                        {
                            child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, Constants.SplitterSpan)));
                            offset += Constants.SplitterSpan;
                        }
                        else
                        {
                            child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, (child as ILayoutSize).DesiredHeight)));
                            offset += (child as ILayoutSize).DesiredHeight;
                        }
                    }
                    break;
            }
            return finalSize;
        }

        private Size _ClipToBounds_Arrange(Size finalSize)
        {
            double avaLength, offset = 0, minLength;
            switch (Direction)
            {
                case Direction.LeftToRight:
                    avaLength = finalSize.Width;

                    foreach (FrameworkElement child in InternalChildren)
                    {
                        if (child is LayoutDragSplitter)
                        {
                            if (avaLength >= Constants.SplitterSpan)
                            {
                                child.Arrange(new Rect(new Point(offset, 0), new Size(Constants.SplitterSpan, finalSize.Height)));
                                offset += Constants.SplitterSpan;
                                avaLength -= Constants.SplitterSpan;
                            }
                            else
                            {
                                child.Arrange(new Rect(new Point(offset, 0), new Size(avaLength, finalSize.Height)));
                                offset += avaLength;
                                avaLength = 0;
                            }
                        }
                        else
                        {
                            minLength = _GetMinLength(child);
                            if (avaLength >= minLength)
                            {
                                child.Arrange(new Rect(new Point(offset, 0), new Size(minLength, finalSize.Height)));
                                offset += minLength;
                                avaLength -= minLength;
                            }
                            else
                            {
                                child.Arrange(new Rect(new Point(offset, 0), new Size(avaLength, finalSize.Height)));
                                offset += avaLength;
                                avaLength = 0;
                            }
                        }
                    }
                    break;
                case Direction.UpToDown:
                    avaLength = finalSize.Height;

                    foreach (FrameworkElement child in InternalChildren)
                    {
                        if (child is LayoutDragSplitter)
                        {
                            if (avaLength >= Constants.SplitterSpan)
                            {
                                child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, Constants.SplitterSpan)));
                                offset += Constants.SplitterSpan;
                                avaLength -= Constants.SplitterSpan;
                            }
                            else
                            {
                                child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, avaLength)));
                                offset += avaLength;
                                avaLength = 0;
                            }
                        }
                        else
                        {
                            minLength = _GetMinLength(child);
                            if (avaLength >= minLength)
                            {
                                child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, minLength)));
                                offset += minLength;
                                avaLength -= minLength;
                            }
                            else
                            {
                                child.Arrange(new Rect(new Point(0, offset), new Size(finalSize.Width, avaLength)));
                                offset += avaLength;
                                avaLength = 0;
                            }
                        }
                    }
                    break;
            }

            return finalSize;
        }

        #endregion

        #region DragSplitter
        private Popup _dragPopup;
        private Point pToScreen;
        private double _dragBound1;
        private double _dragBound2;


        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _ComputeDragBounds(sender as LayoutDragSplitter, ref _dragBound1, ref _dragBound2);
            _CreateDragPopup(sender as LayoutDragSplitter);
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Direction == Direction.LeftToRight)
            {
                if (e.HorizontalChange != 0)
                {
                    double newPos = pToScreen.X + e.HorizontalChange;
                    if (_dragBound1 + Constants.SideLength >= _dragBound2 - Constants.SideLength) return;
                    if ((newPos >= _dragBound1 + Constants.SideLength) && (newPos <= _dragBound2 - Constants.SideLength))
                        _dragPopup.HorizontalOffset = newPos;
                    else
                    {
                        if (e.HorizontalChange > 0)
                            _dragPopup.HorizontalOffset = _dragBound2 - Constants.SideLength;
                        else _dragPopup.HorizontalOffset = _dragBound1 + Constants.SideLength;
                    }
                }
            }
            else
            {
                if (e.VerticalChange != 0)
                {
                    double newPos = pToScreen.Y + e.VerticalChange;
                    if (_dragBound1 + Constants.SideLength >= _dragBound2 - Constants.SideLength) return;
                    if ((newPos >= _dragBound1 + Constants.SideLength) && (newPos <= _dragBound2 - Constants.SideLength))
                        _dragPopup.VerticalOffset = newPos;
                    else
                    {
                        if (e.VerticalChange > 0)
                            _dragPopup.VerticalOffset = _dragBound2 - Constants.SideLength;
                        else _dragPopup.VerticalOffset = _dragBound1 + Constants.SideLength;
                    }
                }
            }
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            double delta = Direction == Direction.LeftToRight ? _dragPopup.HorizontalOffset - pToScreen.X : _dragPopup.VerticalOffset - pToScreen.Y;
            double span1 = 0, span2 = 0;
            var index = Children.IndexOf(sender as UIElement);
            span1 = _GetMinLength(Children[index - 1]);
            span2 = _GetMinLength(Children[index + 1]);
            if (delta != 0)
            {
                if (Direction == Direction.LeftToRight)
                {
                    if ((_dragPopup.HorizontalOffset >= _dragBound1 + span1) && (_dragPopup.HorizontalOffset <= _dragBound2 - span2))
                    {
                        (Children[index - 1] as ILayoutSize).DesiredWidth += delta;
                        (Children[index + 1] as ILayoutSize).DesiredWidth -= delta;
                    }
                    else
                    {
                        if (delta > 0)
                        {
                            (Children[index - 1] as ILayoutSize).DesiredWidth += _dragBound2 - span2 - pToScreen.X;
                            (Children[index + 1] as ILayoutSize).DesiredWidth -= _dragBound2 - span2 - pToScreen.X;
                        }
                        else
                        {
                            (Children[index - 1] as ILayoutSize).DesiredWidth += _dragBound1 + span1 - pToScreen.X;
                            (Children[index + 1] as ILayoutSize).DesiredWidth -= _dragBound1 + span1 - pToScreen.X;
                        }
                    }
                }
                else
                {
                    if ((_dragPopup.VerticalOffset >= _dragBound1 + span1) && (_dragPopup.VerticalOffset <= _dragBound2 - span2))
                    {
                        (Children[index - 1] as ILayoutSize).DesiredHeight += delta;
                        (Children[index + 1] as ILayoutSize).DesiredHeight -= delta;
                    }
                    else
                    {
                        if (delta > 0)
                        {
                            (Children[index - 1] as ILayoutSize).DesiredHeight += _dragBound2 - span2 - pToScreen.Y;
                            (Children[index + 1] as ILayoutSize).DesiredHeight -= _dragBound2 - span2 - pToScreen.Y;
                        }
                        else
                        {
                            (Children[index - 1] as ILayoutSize).DesiredHeight += _dragBound1 + span1 - pToScreen.Y;
                            (Children[index + 1] as ILayoutSize).DesiredHeight -= _dragBound1 + span1 - pToScreen.Y;
                        }
                    }
                }
            }

            _DisposeDragPopup();

            InvalidateMeasure();
        }

        /// <summary>
        /// 计算拖动时的上下边界值
        /// </summary>
        /// <param name="splitter">拖动的对象</param>
        /// <param name="x1">下界</param>
        /// <param name="x2">上界</param>
        private void _ComputeDragBounds(LayoutDragSplitter splitter, ref double x1, ref double x2)
        {
            int index = Children.IndexOf(splitter);
            if (index > 1)
            {
                var splitter_x1 = Children[index - 2];
                var pToScreen = this.PointToScreenDPIWithoutFlowDirection(new Point());
                var transfrom = splitter_x1.TransformToAncestor(this);
                var _pToInterPanel = transfrom.Transform(new Point(0, 0));
                pToScreen.X += _pToInterPanel.X;
                pToScreen.Y += _pToInterPanel.Y;
                if (Direction == Direction.LeftToRight)
                    x1 = pToScreen.X + Constants.SplitterSpan;
                else x1 = pToScreen.Y + Constants.SplitterSpan;
            }
            else
            {
                var pToScreen = this.PointToScreenDPIWithoutFlowDirection(new Point());
                if (Direction == Direction.LeftToRight)
                    x1 = pToScreen.X;
                else x1 = pToScreen.Y;
            }

            if (index < Children.Count - 2)
            {
                var splitter_x2 = Children[index + 2];
                var pToScreen = this.PointToScreenDPIWithoutFlowDirection(new Point());
                var transfrom = splitter_x2.TransformToAncestor(this);
                var _pToInterPanel = transfrom.Transform(new Point(0, 0));
                pToScreen.X += _pToInterPanel.X;
                pToScreen.Y += _pToInterPanel.Y;
                if (Direction == Direction.LeftToRight)
                    x2 = pToScreen.X - Constants.SplitterSpan;
                else x2 = pToScreen.Y - Constants.SplitterSpan;
            }
            else
            {
                var pToScreen = this.PointToScreenDPIWithoutFlowDirection(new Point());
                if (Direction == Direction.LeftToRight)
                    x2 = pToScreen.X + ActualWidth - Constants.SplitterSpan;
                else x2 = pToScreen.Y + ActualHeight - Constants.SplitterSpan;
            }
        }

        private void _CreateDragPopup(LayoutDragSplitter splitter)
        {
            pToScreen = this.PointToScreenDPIWithoutFlowDirection(new Point());
            var transfrom = splitter.TransformToAncestor(this);
            var _pToInterPanel = transfrom.Transform(new Point(0, 0));
            pToScreen.X += _pToInterPanel.X;
            pToScreen.Y += _pToInterPanel.Y;

            int index = Children.IndexOf(splitter);
            switch (Direction)
            {
                case Direction.LeftToRight:
                    (Children[index - 1] as ILayoutSize).DesiredWidth = (Children[index - 1] as FrameworkElement).ActualWidth;
                    (Children[index + 1] as ILayoutSize).DesiredWidth = (Children[index + 1] as FrameworkElement).ActualWidth;
                    break;
                case Direction.UpToDown:
                    (Children[index - 1] as ILayoutSize).DesiredHeight = (Children[index - 1] as FrameworkElement).ActualHeight;
                    (Children[index + 1] as ILayoutSize).DesiredHeight = (Children[index + 1] as FrameworkElement).ActualHeight;
                    break;
            }

            _dragPopup = new Popup()
            {
                Child = new Rectangle()
                {
                    Height = splitter.ActualHeight,
                    Width = splitter.ActualWidth,
                    Fill = Brushes.Black,
                    Opacity = Constants.DragOpacity,
                    IsHitTestVisible = false,
                },
                Placement = PlacementMode.Absolute,
                HorizontalOffset = pToScreen.X,
                VerticalOffset = pToScreen.Y,
                AllowsTransparency = true
            };

            DockHelper.ComputeSpliterLocation(_dragPopup, pToScreen, new Size(splitter.ActualWidth, splitter.ActualHeight));
            _dragPopup.IsOpen = true;
        }

        private void _DisposeDragPopup()
        {
            _dragPopup.IsOpen = false;
            _dragPopup = null;
        }

        internal LayoutDragSplitter _CreateSplitter(Cursor cursor)
        {
            var splitter = new LayoutDragSplitter()
            {
                Cursor = cursor,
                Background = Direction == Direction.LeftToRight ? ResourceManager.SplitterBrushVertical : ResourceManager.SplitterBrushHorizontal,
            };
            splitter.DragStarted += OnDragStarted;
            splitter.DragDelta += OnDragDelta;
            splitter.DragCompleted += OnDragCompleted;
            return splitter;
        }

        internal void _DesstroySplitter(LayoutDragSplitter splitter)
        {
            splitter.DragStarted -= OnDragStarted;
            splitter.DragDelta -= OnDragDelta;
            splitter.DragCompleted -= OnDragCompleted;
        }
        #endregion

        #region Auxiliary Method
        /// <summary>
        /// 判断Child是否包含文档（文档区域大小判定方式为Auto）
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private bool _IsDocumentChild(object child)
        {
            return child is LayoutDocumentGroupControl
                   || (child is LayoutGroupPanel && (child as LayoutGroupPanel).ContainDocument);
        }


        /// <summary>
        /// 计算obj的中最小长度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private double _GetMinLength(object obj)
        {
            if (obj is LayoutGroupPanel && (obj as LayoutGroupPanel).ContainDocument)
            {
                var child = obj as LayoutGroupPanel;
                if (child.Direction == Direction)
                {
                    double length = 0;
                    foreach (var child_ele in child.Children)
                        length += _GetMinLength(child_ele);
                    return length;
                }
                else
                {
                    //若方向不同则找出子元素中最小长度最大的值
                    double max = 0;
                    foreach (var child_ele in child.Children)
                    {
                        var value = _GetMinLength(child_ele);
                        if (max < value)
                            max = value;
                    }
                    return max;
                }
            }
            else
            {
                if (obj is LayoutDragSplitter)
                    return Constants.SplitterSpan;
                else return Constants.SideLength;
            }
        }
        #endregion

        public virtual void AttachChild(IDockView child, AttachMode mode, int index)
        {
            if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("index out of range!");
            if (!_AssertMode(mode)) throw new ArgumentException("mode is illegal!");

            bool flag = true;

            switch (Direction)
            {
                case Direction.None:
                    if (mode == AttachMode.Left || mode == AttachMode.Right)
                        Direction = Direction.LeftToRight;
                    else Direction = Direction.UpToDown;
                    break;
                case Direction.LeftToRight:
                    if (mode == AttachMode.Left || mode == AttachMode.Right)
                    {
                        if (child is LayoutGroupPanel && !(child as LayoutGroupPanel).IsDocumentPanel)
                        {
                            flag = false;
                            _AttachSubPanel((child as LayoutGroupPanel), index);
                        }
                    }
                    else
                    {
                        if (IsRootPanel)
                        {
                            flag = false;
                            AttachToRootPanel(child, mode);
                        }
                        else throw new ArgumentException("mode is illegal!");
                    }
                    break;
                case Direction.UpToDown:
                    if (mode == AttachMode.Top || mode == AttachMode.Bottom)
                    {
                        if (child is LayoutGroupPanel && !(child as LayoutGroupPanel).IsDocumentPanel)
                        {
                            flag = false;
                            _AttachSubPanel((child as LayoutGroupPanel), index);
                        }
                    }
                    else
                    {
                        if (IsRootPanel)
                        {
                            flag = false;
                            AttachToRootPanel(child, mode);
                        }
                        else throw new ArgumentException("mode is illegal!");
                    }
                    break;
            }
            if (flag)
                _AttachChild(child, index);
        }

        public virtual void DetachChild(IDockView child, bool force = true)
        {
            if (Children.Contains(child as UIElement))
            {
                _DetachChild(child);
                if (force)
                {
                    if (Children.Count < 2)
                    {
                        Direction = Direction.None;
                        _isAnchorPanel = false;
                    }

                    //若元素为空，且不是RootPanel，则递归的将其从Parent中移除
                    if (Children.Count == 0 && !IsRootPanel)
                    {
                        (Parent as ILayoutViewParent).DetachChild(this);
                        Dispose();
                    }
                    //若元素只有一个，则将子元素层次减一
                    if (Children.Count == 1)
                    {
                        child = Children[0] as IDockView;
                        var parent = Parent as ILayoutViewParent;
                        var index = parent.IndexOf(this);
                        //从父容器中移除自己
                        parent.DetachChild(this, false);
                        Dispose();
                        //从父容器中加入自己的子元素
                        if (parent is LayoutGroupPanel)
                        {
                            switch ((parent as LayoutGroupPanel).Direction)
                            {
                                case Direction.None:
                                    parent.AttachChild(child, AttachMode.None, Math.Max(index - 1, 0));
                                    break;
                                case Direction.LeftToRight:
                                    parent.AttachChild(child, AttachMode.Left, Math.Max(index - 1, 0));
                                    break;
                                case Direction.UpToDown:
                                    parent.AttachChild(child, AttachMode.Top, Math.Max(index - 1, 0));
                                    break;
                            }
                        }
                        else parent.AttachChild(child, AttachMode.None, Math.Max(index - 1, 0));
                    }
                }
            }
        }

        public int IndexOf(IDockView child)
        {
            if (Children.Contains(child as UIElement))
                return Children.IndexOf(child as UIElement);
            else return -1;
        }

        internal void _AttachChild(IDockView child, int index)
        {
            //DockManager.ChangeDockMode(child, DockManager == null ? DockMode.Float : DockMode.Normal);
            Children.Insert(index, child as UIElement);
            if (Children.Count > 1)
            {
                switch (Direction)
                {
                    case Direction.LeftToRight:
                        if (index % 2 == 0)
                            Children.Insert(index + 1, _CreateSplitter(Cursors.SizeWE));
                        else Children.Insert(index, _CreateSplitter(Cursors.SizeWE));
                        break;
                    case Direction.UpToDown:
                        if (index % 2 == 0)
                            Children.Insert(index + 1, _CreateSplitter(Cursors.SizeNS));
                        else Children.Insert(index, _CreateSplitter(Cursors.SizeNS));
                        break;
                }
            }
        }

        internal void _DetachChild(IDockView child)
        {
            if (Children.Contains(child as UIElement))
            {
                int index = Children.IndexOf(child as UIElement);
                if (index > 0)
                {
                    Children.RemoveAt(index);
                    //移除对应的Spliter
                    if (Children.Count > 0)
                    {
                        _DesstroySplitter(Children[index - 1] as LayoutDragSplitter);
                        Children.RemoveAt(index - 1);
                    }
                }
                else
                {
                    Children.RemoveAt(0);
                    //移除对应的Spliter
                    if (Children.Count > 0)
                    {
                        _DesstroySplitter(Children[0] as LayoutDragSplitter);
                        Children.RemoveAt(0);
                    }
                }
            }
        }

        protected void _AttachSubPanel(LayoutGroupPanel subpanel, int index)
        {
            List<UIElement> children = new List<UIElement>();
            foreach (UIElement ele in subpanel.Children)
                children.Add(ele);
            children.Reverse();
            subpanel.Children.Clear();
            foreach (UIElement ele in children)
                if (ele is IDockView)
                    _AttachChild(ele as IDockView, index);
        }

        internal void AttachToRootPanel(IDockView child, AttachMode mode)
        {
            var parent = Parent as ILayoutViewParent;
            parent.DetachChild(this);
            var pparent = new LayoutGroupPanel()
            {
                Direction = (mode == AttachMode.Left || mode == AttachMode.Right) ? Direction.LeftToRight : Direction.UpToDown
            };
            parent.AttachChild(pparent, AttachMode.None, 0);
            if (mode == AttachMode.Left || mode == AttachMode.Top)
            {
                pparent._AttachChild(this, 0);
                pparent._AttachChild(child, 0);
            }
            else
            {
                pparent._AttachChild(child, 0);
                pparent._AttachChild(this, 0);
            }
        }

        internal virtual bool _AssertMode(AttachMode mode)
        {
            return mode == AttachMode.Left
                || mode == AttachMode.Top
                || mode == AttachMode.Right
                || mode == AttachMode.Bottom;
        }

        #region Drag
        private DropMode _dropMode = DropMode.None;
        public DropMode DropMode
        {
            get { return _dropMode; }
            set { _dropMode = value; }
        }
        DropWindow _dragWnd;
        public void OnDrop(DragItem source)
        {
            var child = (source.RelativeObj as BaseFloatWindow).Child;
            (source.RelativeObj as BaseFloatWindow).DetachChild(child);
            DockManager.ChangeDockMode(child, DockMode.Normal);
            DockManager.FormatChildSize(child as ILayoutSize, new Size(ActualWidth, ActualHeight));
            //取消AttachObj信息
            DockManager.ClearAttachObj(child);


            switch (DropMode)
            {
                case DropMode.Left:
                    DockManager.ChangeSide(child, DockSide.Left);
                    AttachChild(child, AttachMode.Left, 0);
                    break;
                case DropMode.Top:
                    DockManager.ChangeSide(child, DockSide.Top);
                    AttachChild(child, AttachMode.Top, 0);
                    break;
                case DropMode.Right:
                    DockManager.ChangeSide(child, DockSide.Right);
                    AttachChild(child, AttachMode.Right, Count);
                    break;
                case DropMode.Bottom:
                    DockManager.ChangeSide(child, DockSide.Bottom);
                    AttachChild(child, AttachMode.Bottom, Count);
                    break;
            }

            (source.RelativeObj as BaseFloatWindow).Close();
        }

        public void CreateDropWindow()
        {
            if (_dragWnd == null)
                _dragWnd = new DropWindow(this);
        }

        public void CloseDropWindow()
        {
            if (_dragWnd != null)
            {
                _dropMode = DropMode.None;
                _dragWnd.Close();
                _dragWnd = null;
            }
        }

        public void HideDropWindow()
        {
            _dragWnd?.Hide();
        }

        public void ShowDropWindow()
        {
            if (_dragWnd == null)
                CreateDropWindow();
            var p = this.PointToScreenDPIWithoutFlowDirection(new Point());
            DockHelper.UpdateLocation(_dragWnd, p.X, p.Y, ActualWidth, ActualHeight);
            if (!_dragWnd.IsOpen) _dragWnd.IsOpen = true;
            _dragWnd.Show();
        }

        public void Update(Point mouseP)
        {
            _dragWnd?.Update(mouseP);
        }
        #endregion

        public event EventHandler Disposed = delegate { };

        public void Dispose()
        {
            Children.Clear();
            Disposed(this ,new EventArgs());
        }
    }
}