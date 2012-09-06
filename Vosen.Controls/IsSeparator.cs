using System;
using System.Windows.Data;
using System.Windows.Controls;

namespace Vosen.Controls
{
	[ValueConversion(typeof(ContentPresenter), typeof(bool))]
	public class IsSeparator : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value is Separator;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
