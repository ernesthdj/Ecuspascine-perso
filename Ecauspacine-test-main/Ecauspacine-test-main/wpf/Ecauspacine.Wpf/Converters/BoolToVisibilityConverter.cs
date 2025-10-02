﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ecauspacine.Wpf.Converters;

/// <summary>
/// Convertit un bool -> Visibility (true => Visible, false => Collapsed).
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && b ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility v && v == Visibility.Visible;
}
