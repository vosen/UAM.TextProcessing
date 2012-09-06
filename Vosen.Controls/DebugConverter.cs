using System;
using System.Windows.Data;

namespace Vosen.Controls
{
	[ValueConversion(typeof(object), typeof(object))]
	public class DebugConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}
