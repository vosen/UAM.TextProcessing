using System.Windows.Controls;

namespace Vosen.Controls
{
	// This class is a dirty hack to force grid grouping to work, dont touch
	internal class GroupingStackPanel : StackPanel
	{
		protected override void OnRender(System.Windows.Media.DrawingContext dc)
		{
			base.OnRender(dc);
			if(!Grid.GetIsSharedSizeScope(this))
				Grid.SetIsSharedSizeScope(this, true);
		}
	}
}
