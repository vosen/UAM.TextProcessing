using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Vosen.SQLFilter;

namespace SQLFilter.FilterView.Test
{
    [ValueConversion(typeof(string), typeof(string))]
    public class PlainStringExprConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = value as string;
            if (text == null)
                return Binding.DoNothing;
            return Filter.Unescape(text);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = value as string;
            if (text == null)
                return Binding.DoNothing;
            return Filter.Escape(text);
        }
    }
}
