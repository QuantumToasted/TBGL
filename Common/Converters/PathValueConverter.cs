using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Platform.Storage;
using TBGL.Extensions;

namespace TBGL.Common;

public class PathValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not int length)
            return BindingOperations.DoNothing;

        string? path = value switch
        {
            IStorageFile { Path.AbsolutePath: var filePath } => filePath,
            XLWorksheetLoadResult { Path: var resultPath } => resultPath,
            _ => null
        };

        if (string.IsNullOrWhiteSpace(path))
            return BindingOperations.DoNothing;

        return DecodeUriString(path).TruncateFromEnd(length);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }

    private static string DecodeUriString(string url)
    {
        string newUrl;
        while ((newUrl = Uri.UnescapeDataString(url)) != url)
            url = newUrl;

        return newUrl;
    }
}