using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DodocoTales.SR.Gui.Converters
{
    public class DDCVProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var now = System.Convert.ToDouble(values[0]);
            var max = System.Convert.ToDouble(values[1]);
            return max <= 0 ? 0: now * 100.0 / max;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}