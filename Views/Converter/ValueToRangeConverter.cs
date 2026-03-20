using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UmweltMonitor3000.Views.Converter;

public class ValueToRangeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int valueToConvert)
            return "";

        if (valueToConvert <= 30)
            return "Low";

        if (valueToConvert <= 70)
            return "Mid";

        return "High";

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
