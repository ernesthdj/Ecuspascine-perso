using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ecauspacine.Wpf.Converters;

public class StringNullOrEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var hasValue = value is string s && !string.IsNullOrWhiteSpace(s);
        return hasValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
