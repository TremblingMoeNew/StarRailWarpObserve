using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DodocoTales.SR.Gui.Converters
{
    public class DDCVPercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue) return DependencyProperty.UnsetValue;
            var divident = System.Convert.ToInt32(values[0]);
            var divisor = System.Convert.ToInt32(values[1]);
            return divisor == 0 ? " — % " : string.Format("{0:P2}", divident * 1.0 / divisor);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
