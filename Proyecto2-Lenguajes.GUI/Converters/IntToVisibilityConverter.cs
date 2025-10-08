using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Proyecto2_Lenguajes.GUI.Converters;

public class IntToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int errores && parameter is string umbralStr && int.TryParse(umbralStr, out int umbral))
        {
            return errores >= umbral;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
