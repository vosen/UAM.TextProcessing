using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Vosen.Controls
{
	[ValueConversion(typeof(Color), typeof(Color))]
	public class ScreenColors : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is Color) || !(parameter is Color))
				return null;
			return ColorHelper.BlendScreen((Color)value, (Color)parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
