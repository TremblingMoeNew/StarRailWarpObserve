using DodocoTales.SR.Library;
using DodocoTales.SR.Library.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DodocoTales.SR.Gui.Converters
{
    public class DDCVClientTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DDCLGameClientType type = (DDCLGameClientType)value;
            return DDCL.GetGameClientTypeName(type);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
