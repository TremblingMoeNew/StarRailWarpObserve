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
    public class DDCVTimeProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var begin = ((DateTimeOffset)values[0]).UtcTicks;
            var end = ((DateTimeOffset)values[1]).UtcTicks;
            var now = (DateTimeOffset.Now).UtcTicks;
            if (now > end || now < begin) return 0.0;
            return (now - begin) * 100.0 / (end - begin);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
