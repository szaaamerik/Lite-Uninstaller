using System.Globalization;
using System.Windows.Data;

namespace Lite_Uninstaller.Helpers;

public class DivideConverter : IValueConverter
{
    private readonly Dictionary<Tuple<object, object>, object> _cache = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var key = Tuple.Create(value, parameter);

        if (_cache.TryGetValue(key, out var result))
        {
            return result;
        }

        if (!double.TryParse(value.ToString(), out var dividend) ||
            !double.TryParse(parameter.ToString(), out var divisor) || divisor == 0)
        {
            return Binding.DoNothing;
        }

        var computedResult = dividend / divisor;
        return _cache[key] = computedResult;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}