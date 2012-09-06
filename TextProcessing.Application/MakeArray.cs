using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SQLFilter.FilterView.Test
{
    [ValueConversion(typeof(object), typeof(object[]))]
    public class MakeArray : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[] { value };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object[] arr = value as object[];
            if (arr == null || arr.Length == 0)
                return Binding.DoNothing;
            return arr[0];
        }
    }
}
