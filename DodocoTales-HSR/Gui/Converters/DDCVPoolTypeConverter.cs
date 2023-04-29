using DodocoTales.SR.Common.Enums;
using DodocoTales.SR.Library;
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
    public class DDCVPoolTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DDCCPoolType type = (DDCCPoolType)value;
            return DDCL.GetPoolTypeName(type);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
