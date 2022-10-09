using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Pickle.TransferLite.ViewModels;

namespace Pickle.TransferLite.Converters;

public class FileEntityToImageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            DirectoryViewModel => (DrawingImage)Application.Current!.FindResource("FolderDrawingImage")!,
            FileViewModel => (DrawingImage)Application.Current!.FindResource("DocumentDrawingImage")!,
            _ => new DrawingImage()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}