using System;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace Vosen.Controls
{
	[ValueConversion(typeof(object), typeof(bool))]
	internal class RootLines : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DependencyObject obj = value as DependencyObject;
			if (obj == null)
				return Visibility.Visible;
			TreeView root = ItemsControl.ItemsControlFromItemContainer(obj) as TreeView;
			if (root == null)
				return Visibility.Visible;
			if (root.ShowRootLines)
				return Visibility.Visible;
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new NotImplementedException();
		}
	}
}
