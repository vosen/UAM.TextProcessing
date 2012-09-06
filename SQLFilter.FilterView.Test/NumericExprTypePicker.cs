using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Vosen.SQLFilter;

namespace SQLFilter.FilterView.Test
{
    [ValueConversion(typeof(int), typeof(int))]
    public class NumericExprTypePicker : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? type = value as int?;
            switch(type)
            {
                default:
                case SQLFilterLexer.EQUALS:
                    return 0;
                case SQLFilterLexer.NOTEQUALS:
                    return 1;
                case SQLFilterLexer.GREATER:
                    return 2;
                case SQLFilterLexer.GREATEROREQUALS:
                    return 3;
                case SQLFilterLexer.LESSER:
                    return 4;
                case SQLFilterLexer.LESSEROREQUALS:
                    return 5;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? selection = value as int?;
            switch (selection)
            {
                default:
                case 0:
                    return SQLFilterLexer.EQUALS;
                case 1:
                    return SQLFilterLexer.NOTEQUALS;
                case 2:
                    return SQLFilterLexer.GREATER;
                case 3:
                    return SQLFilterLexer.GREATEROREQUALS;
                case 4:
                    return SQLFilterLexer.LESSER;
                case 5:
                    return SQLFilterLexer.LESSEROREQUALS;
            }
        }
    }
}
