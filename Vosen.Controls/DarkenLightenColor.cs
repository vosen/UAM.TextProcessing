using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Vosen.Controls
{
	[ValueConversion(typeof(Color), typeof(Color))]
	public class DarkenLightenColor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is Color) || !(parameter is float))
				return null;
			Color color = (Color)value;
			int h, i, s;
			ColorHelper.Color2HBS(color, out h, out i, out s);
			if (i >= 128)
				return ColorHelper.Darken(color, (float)parameter);
			return ColorHelper.Lighten(color, (float)parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
