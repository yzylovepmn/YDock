using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using YDock.Enum;

namespace YDock.Converters
{
    [ValueConversion(typeof(DockMode), typeof(Transform))]
    public class DockModeToTransformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DockMode mode = (DockMode)value;

            if (mode == DockMode.DockBar)
                return new RotateTransform(90);

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}