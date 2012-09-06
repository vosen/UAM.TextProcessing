using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Vosen.SQLFilter;

namespace SQLFilter.FilterView.Test
{
    [ValueConversion(typeof(int), typeof(string[]))]
    public class EnumGenerator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string prop = value as string;
            if (prop == null)
                return Binding.DoNothing;
            return MainWindow.GenerateEnums(prop);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
