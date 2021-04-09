using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
namespace TTApplication.Converters
{
    /// <summary>
    /// Value converter that translates string to <see cref="Color"/>
    /// </summary>
    public sealed class StringToColorConverter :  IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var hexValue = (string)value;
            return FromHexString(hexValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var color = (Color)value;
            return $"{color.A}{color.R}{color.G}{color.B}";
        }

        private static Color FromHexString(string hexValue)
        {
            try
            {

                byte a = 255;
                var startPos = 0;
                hexValue = hexValue.Replace("#","");
                if (hexValue.Length == 8)
                {
                    a = System.Convert.ToByte(hexValue.Substring(startPos, 2), 16);
                    startPos += 2;
                }
                var r = System.Convert.ToByte(hexValue.Substring(startPos , 2), 16);
                var g = System.Convert.ToByte(hexValue.Substring(startPos +2, 2), 16);
                var b = System.Convert.ToByte(hexValue.Substring(startPos +4, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
            catch { }
            return Colors.Transparent;
        }
    }
}
