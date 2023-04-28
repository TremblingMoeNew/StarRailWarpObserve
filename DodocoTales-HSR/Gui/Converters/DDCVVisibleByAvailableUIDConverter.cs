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
    public class DDCVVisibleByAvailableUIDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = Visibility.Collapsed;
            try
            {
                var uid = System.Convert.ToInt64(value);
                if (uid >= 0) res = Visibility.Visible;
            }
            catch { }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
