using System.Globalization;

public class BrushToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush solidColorBrush)
        {
            return solidColorBrush.Color;
        }
        // Return a default color if the brush is not a SolidColorBrush
        else if(parameter != null && parameter is BoxView boxView)
            return boxView.Color;
        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
