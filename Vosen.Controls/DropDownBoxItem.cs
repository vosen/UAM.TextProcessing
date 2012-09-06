using System.Windows;
using System.Windows.Controls;

namespace Vosen.Controls
{
	public class DropDownBoxItem : ComboBoxItem
	{
		static DropDownBoxItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownBoxItem), new FrameworkPropertyMetadata(typeof(DropDownBoxItem)));
		}

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register(
				"Icon",
				typeof(object),
				typeof(DropDownBoxItem),
				new FrameworkPropertyMetadata(null));
	

		public object Icon
		{
			get
			{
				return GetValue(IconProperty);
			}
			set
			{
				SetValue(IconProperty, value);
			}
		}

	}
}