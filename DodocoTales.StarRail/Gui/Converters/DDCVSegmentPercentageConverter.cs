using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace DodocoTales.SR.Gui.Converters
{
    public class DDCVSegmentPercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue) return DependencyProperty.UnsetValue;
            var divident = System.Convert.ToInt32(values[0]);
            var divisor = System.Convert.ToInt32(values[1]);
            if (divisor == 0) return "---";
            var res = divident * 100.0 / divisor;
            if (res >= 100.0) return "100";
            else if (res >= 10.0) return string.Format("{0:F1}", res);
            else return string.Format("{0:F2}", res);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
