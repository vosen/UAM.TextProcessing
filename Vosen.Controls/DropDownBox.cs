using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Vosen.Controls
{
	public class DropDownBox : ComboBox
	{
		static DropDownBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownBox), new FrameworkPropertyMetadata(typeof(DropDownBox)));
		}

		public static readonly DependencyProperty ContentBoxTemplateProperty =
			DependencyProperty.Register(
				"ContentBoxTemplate",
				typeof(DataTemplate),
				typeof(DropDownBox),
				new FrameworkPropertyMetadata(null));

		// This is the same as SelectionBoxItemTemplate but overridable
		public DataTemplate ContentBoxTemplate
		{
			get { return (DataTemplate)GetValue(ContentBoxTemplateProperty); }
			set { SetValue(ContentBoxTemplateProperty, value); }
		}

		public static readonly DependencyProperty ContentBoxForegroundProperty =
			DependencyProperty.Register(
				"ContentBoxForeground",
				typeof(Brush),
				typeof(DropDownBox),
				new FrameworkPropertyMetadata(SystemColors.ControlTextBrush));

		// Color for control box text
		public Brush ContentBoxForeground
		{
			get { return (Brush)GetValue(ContentBoxForegroundProperty); }
			set { SetValue(ContentBoxForegroundProperty, value); }
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new DropDownBoxItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return (item is DropDownBoxItem);
		}

	}
}