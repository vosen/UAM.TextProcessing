using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Vosen.SQLFilter;
using System.Windows.Data;

namespace SQLFilter.FilterView.Test
{
    [ValueConversion(typeof(int), typeof(HierarchicalDataTemplate))]
    class NodeTemplateSelector : IValueConverter
    {
        public HierarchicalDataTemplate IPTemplate { get; set; }
        public HierarchicalDataTemplate BoolTemplate { get; set; }
        public HierarchicalDataTemplate EnumTemplate { get; set; }
        public HierarchicalDataTemplate NumericTemplate { get; set; }
        public HierarchicalDataTemplate StringTemplate { get; set; }

        public HierarchicalDataTemplate GroupTemplate { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is int))
                return null;
            switch ((int)value)
            {
                case SQLFilterLexer.IP_EXPR:
                    return IPTemplate;
                case SQLFilterLexer.BOOL_EXPR:
                    return BoolTemplate;
                case SQLFilterLexer.ENUM_EXPR:
                    return EnumTemplate;
                case SQLFilterLexer.NUM_EXPR:
                    return NumericTemplate;
                case SQLFilterLexer.STRING_EXPR:
                    return StringTemplate;
                case SQLFilterLexer.AND:
                case SQLFilterLexer.OR:
                case SQLFilterLexer.NOT_AND:
                case SQLFilterLexer.NOT_OR:
                    return GroupTemplate;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
