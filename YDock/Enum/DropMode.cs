using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YDock.Enum
{
    [Flags]
    public enum DropMode
    {
        None = 0x0000,
        Header = 0x0001,
        Left = 0x0002,
        Top = 0x0004,
        Right = 0x0008,
        Bottom = 0x0010,
        Left_WithSplit = Left | Split,
        Top_WithSplit = Top | Split,
        Right_WithSplit = Right | Split,
        Bottom_WithSplit = Bottom | Split,
        Center = 0x0020,
        Split = 0x1000,
    }

    public enum AttachMode
    {
        None,
        Left,
        Top,
        Right,
        Bottom,
        Left_WithSplit,
        Top_WithSplit,
        Right_WithSplit,
        Bottom_WithSplit,
        Center
    }
}