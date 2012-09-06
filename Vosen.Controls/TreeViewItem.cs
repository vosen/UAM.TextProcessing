using System.Windows;
using System.Windows.Input;

namespace Vosen.Controls
{
	public class TreeViewItem : System.Windows.Controls.TreeViewItem
	{

		static TreeViewItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItem), new FrameworkPropertyMetadata(typeof(TreeViewItem)));
		}

		public static readonly DependencyProperty DoubleClickExpandProperty =
			 DependencyProperty.Register(
				"DoubleClickExpand",
				typeof(bool),
				typeof(TreeViewItem),
				new FrameworkPropertyMetadata(true));

		public bool DoubleClickExpand
		{
			get { return (bool)GetValue(DoubleClickExpandProperty); }
			set { SetValue(DoubleClickExpandProperty, value); }
		}

		// Prevent item from expanding on double click
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (!e.Handled && IsEnabled && !DoubleClickExpand)
			{
				Focus();
				e.Handled = true;
			}
			base.OnMouseLeftButtonDown(e);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new TreeViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return (item is TreeViewItem);
		}

		public bool HiddenRootLine
		{
			get
			{
				TreeView root = ItemsControlFromItemContainer(this) as TreeView;
				if (root == null)
					return false;
				return !root.ShowRootLines;
			}
		}

		public bool ShowLines
		{
			get
			{
				var itemParent = ItemsControlFromItemContainer(this);
				var parent = itemParent as TreeViewItem;
				if (parent != null)
					return parent.ShowLines;
				TreeView root = itemParent as TreeView;
				if (root != null)
					return root.ShowLines;
				return true;
			}
		}
	}
}
