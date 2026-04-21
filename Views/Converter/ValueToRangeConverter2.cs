using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UmweltMonitor3000.Views.Converter;

public class ValueToRangeConverter2 : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int valueToConvert)
            return "";

        if (valueToConvert <= 30)
            return "kritisch";

        if (valueToConvert <= 40)
            return "durstig";

        if (valueToConvert <= 70)
            return "zunass";

        return "optimal";

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Windows.Data.Binding.DoNothing;
    }
}