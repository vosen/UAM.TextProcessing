using System.Windows;

namespace Vosen.Controls
{
	public class TreeView : System.Windows.Controls.TreeView
	{
		public static readonly DependencyProperty ShowLinesProperty =
			 DependencyProperty.Register(
				"ShowLines",
				typeof(bool),
				typeof(TreeView),
				new FrameworkPropertyMetadata(true));

		public bool ShowLines
		{
			get { return (bool)GetValue(ShowLinesProperty); }
			set { SetValue(ShowLinesProperty, value); }
		}

		public static readonly DependencyProperty HidePlusMinusProperty =
			 DependencyProperty.Register(
				"HidePlusMinus",
				typeof(bool),
				typeof(TreeView),
				new FrameworkPropertyMetadata(false));

		public bool HidePlusMinus
		{
			get { return (bool)GetValue(HidePlusMinusProperty); }
			set { SetValue(HidePlusMinusProperty, value); }
		}

		public static readonly DependencyProperty ShowRootLinesProperty =
			 DependencyProperty.Register(
				"ShowRootLines",
				typeof(bool),
				typeof(TreeView),
				new FrameworkPropertyMetadata(false));

		public bool ShowRootLines
		{
			get { return (bool)GetValue(ShowRootLinesProperty); }
			set { SetValue(ShowRootLinesProperty, value); }
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new TreeViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return (item is TreeViewItem);
		}
	}
}
