using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Pickle.TransferLite.Converters;

public class InterestToAngle : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (double)(value ?? 0D) * 360 / 100;

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (double)(value ?? 0D) * 100 / 360;
    }
}