using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Vosen.SQLFilter;

namespace SQLFilter.FilterView.Test
{
    [ValueConversion(typeof(int), typeof(int))]
    public class StringExprTypePicker : IValueConverter
    {
        public static int Convert(int pos)
        {
            switch (pos)
            {
                default:
                case StringPatternLexer.IS:
                    return 0;
                case StringPatternLexer.CONTAINS:
                    return 1;
                case StringPatternLexer.BEGINS:
                    return 2;
                case StringPatternLexer.ENDS:
                    return 3;
                // 4 is separator
                case StringPatternLexer.COMPLEX:
                    return 5;
            }
        }

        public static int ConvertBack(int type)
        {
            switch (type)
            {
                default:
                case 0:
                    return StringPatternLexer.IS;
                case 1:
                    return StringPatternLexer.CONTAINS;
                case 2:
                    return StringPatternLexer.BEGINS;
                case 3:
                    return StringPatternLexer.ENDS;
                // 4 is separator
                case 5:
                    return StringPatternLexer.COMPLEX;
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
