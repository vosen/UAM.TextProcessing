using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Vosen.SQLFilter;

namespace SQLFilter.FilterView.Test
{
    public class OSNameToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sysName = value as string;
            if (sysName == null)
                return 0;

            OperatingSystem os;
            if (!Enum.TryParse(sysName, out os))
                return 0;

            switch (os)
            {
                case OperatingSystem.Windows:
                    return 0;
                case OperatingSystem.Linux:
                    return 1;
                case OperatingSystem.Mac:
                    return 2;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    return OperatingSystem.Windows;
                case 1:
                    return OperatingSystem.Linux;
                case 2:
                    return OperatingSystem.Mac;
            }
            return null;
        }
    }
}
