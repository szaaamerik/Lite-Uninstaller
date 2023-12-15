using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lite_Uninstaller.Helpers;

public class BoolConverter: IMultiValueConverter
{
    public object Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is [bool value1, bool value2])
        {
            return value1 && !value2;
        }

        return DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}