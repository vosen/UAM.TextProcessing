using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Vosen.SQLFilter;

namespace SQLFilter.FilterView.Test
{
    [ValueConversion(typeof(int), typeof(int))]
    public class BoolValuePicker : IValueConverter
    {
        public static int Convert(int pos)
        {
            switch (pos)
            {
                default:
                case SQLFilterLexer.TRUE:
                    return 0;
                case SQLFilterLexer.FALSE:
                    return 1;
            }
        }

        public static int ConvertBack(int type)
        {
            switch (type)
            {
                default:
                case 0:
                    return SQLFilterLexer.TRUE;
                case 1:
                    return SQLFilterLexer.FALSE;
            }
        }


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is int))
                return Binding.DoNothing;
            return Convert((int)value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is int))
                return Binding.DoNothing;
            return ConvertBack((int)value);

        }
    }
}
