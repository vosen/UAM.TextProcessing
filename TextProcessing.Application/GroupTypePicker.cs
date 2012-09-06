using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Vosen.SQLFilter;

namespace SQLFilter.FilterView.Test
{
    [ValueConversion(typeof(int), typeof(int))]
    public class GroupTypePicker : IValueConverter
    {
        public static int Convert(int pos)
        {
            switch (pos)
            {
                default:
                case SQLFilterLexer.AND:
                    return 0;
                case SQLFilterLexer.OR:
                    return 1;
                // 2 is separator
                case SQLFilterLexer.NOT_OR:
                    return 3;
                case SQLFilterLexer.NOT_AND:
                    return 4;
            }
        }

        public static int ConvertBack(int type)
        {
            switch (type)            
            {
                default:
                case 0:
                    return SQLFilterLexer.AND;
                case 1:
                    return SQLFilterLexer.OR;
                // 2 is separator
                case 3:
                    return SQLFilterLexer.NOT_OR;
                case 4:
                    return SQLFilterLexer.NOT_AND;
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
