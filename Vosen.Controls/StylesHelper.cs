using System;
using System.Windows;

namespace Vosen.Controls
{
	static class StylesHelper
	{
		public static readonly DataTemplate EllipsisHeader;
		public static readonly Style EllipsisCell;

		static StylesHelper()
		{
			ResourceDictionary resource = new ResourceDictionary{ Source = new Uri("pack://application:,,,/Vosen.Controls;component/Styles.xaml") };
			EllipsisHeader = (DataTemplate)resource["EllipsisHeader"];
			EllipsisCell = (Style)resource["EllipsisCell"];
		}
	}
}
