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
    public class DDCVTimeZoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timezone = (int)value;
            if (timezone == 0) return "UTC";
            else if (timezone > 0) return "UTC+" + timezone.ToString();
            else return "UTC" + timezone.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;
            if (str == "UTC") return 0;
            else if(str.StartsWith("UTC")) str = str.Substring(3);
            if (Int32.TryParse(str, out int res)) return res;
            return DependencyProperty.UnsetValue;
        }
    }
}
