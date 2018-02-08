using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace YDock.View
{
    public abstract class BaseDropVisual : BaseVisual
    {
        public BaseDropVisual(int flag)
        {
            _flag = flag;
        }

        private int _flag;
        public int Flag
        {
            get { return _flag; }
        }
    }

    public class GlassDropVisual : BaseDropVisual
    {
        public GlassDropVisual(int flag) : base(flag)
        {

        }

        public override void Update(Size size)
        {
            
        }
    }

    public class ActiveRectDropVisual : BaseDropVisual
    {
        public ActiveRectDropVisual(int flag) : base(flag)
        {
        }

        public override void Update(Size size)
        {
            
        }

        private double _length;
        public double Length
        {
            get { return _length; }
            set { _length = value; }
        }

        private double? _headerLength;
        public double? HeaderLength
        {
            get { return _headerLength; }
            set { _headerLength = value; }
        }

        private double? _offset;
        new public double? Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
    }

    public class UnitDropVisual: BaseDropVisual
    {
        public UnitDropVisual(int flag) : base(flag)
        {

        }

        public override void Update(Size size)
        {
            
        }
    }
}