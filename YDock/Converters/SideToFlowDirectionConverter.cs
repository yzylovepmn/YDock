using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using YDock.Enum;

namespace YDock.Converters
{
    [ValueConversion(typeof(DockSide), typeof(FlowDirection))]
    public class SideToFlowDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DockSide side = (DockSide)value;
            if (side == DockSide.Left)
                return FlowDirection.RightToLeft;

            return FlowDirection.LeftToRight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
