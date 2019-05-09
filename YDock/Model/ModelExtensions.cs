using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDock.Enum;

namespace YDock.Model
{
    public static class ModelExtensions
    {
        public static bool Assert(this DockSide side)
        {
            if (side == DockSide.None ||
                side == DockSide.Left ||
                side == DockSide.Right ||
                side == DockSide.Top ||
                side == DockSide.Bottom)
                return true;
            return false;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T ele) where T : class
        {
            var index = -1;
            foreach (var item in source)
            {
                index++;
                if (item == ele)
                    break;
            }
            return index;
        }
    }
}