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

        var path = value switch
        {
            IStorageFile { Path.LocalPath: var filePath } => filePath,
            XLWorksheetLoadResult { Path: var resultPath } => resultPath,
            Uri { LocalPath: var filePath } => filePath,
            _ => null
        };

        if (string.IsNullOrWhiteSpace(path))
            return BindingOperations.DoNothing;

        return path.TruncateFromEnd(length);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}