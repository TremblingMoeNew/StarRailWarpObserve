using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DodocoTales.SR.Gui.Converters
{
    public class DDCVItemNameColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value.ToString());
            var hash = MD5.Create().ComputeHash(bytes);
            var h = (float)(hash.Take(2).Average(x => x) / 256 * 360);
            var s = (float)(hash.Skip(5).Take(5).Average(x => x) / 256 * 50 + 20);
            var l = (float)(hash.Skip(10).Average(x => x) / 256 * 50 + 20);
            var c = SKColor.FromHsl(h, s, l);
            return new SolidColorBrush(Color.FromRgb(c.Red, c.Green, c.Blue));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
