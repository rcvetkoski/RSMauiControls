using System.Globalization;
using System.Text.RegularExpressions;

namespace TestApplicationMaui.Helpers.Converters
{
    public class EnumToFriendlyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Enum enumValue)
            {
                var input = enumValue.ToString();
                return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not supported.");
        }
    }
}
