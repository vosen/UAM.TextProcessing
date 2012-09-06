using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Antlr.Runtime;
using Vosen.SQLFilter;
using Antlr.Runtime.Tree;
using Vosen.Controls;
using System.ComponentModel;

namespace SQLFilter.FilterView.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FilterWindow<FakeServer>
    {
        public MainWindow(Filter<FakeServer> filter)
            : base(filter)
        {
            InitializeComponent();
            FilterObject.PropertyChanged += (src, arg) => UpdateFilter(arg.PropertyName);
        }

        private void PropertyBoxUpdated(object sender, DataTransferEventArgs e)
        {
            DropDownBox src = e.Source as DropDownBox;
            if (src == null)
                return;
            FilterTree tree = src.DataContext as FilterTree;
            if (tree == null)
                return;
            string oldProperty = src.SelectedValue as string;
            if (oldProperty == null)
                return;
            string newProperty = tree.GetChild(0).GetChild(0).Text;
            if (oldProperty != newProperty && newProperty != null)
                UpdateExpression(tree, oldProperty, newProperty);
        }

        private void NodeUpdated(object sender, DataTransferEventArgs e)
        {
            FrameworkElement src = e.Source as FrameworkElement;
            if (src == null)
                return;
            FilterTree tree = src.DataContext as FilterTree;
            if (tree == null)
                return;
            MarkNodeAsUpdated(tree);
        }

        private void OperatorUpdated(object sender, DataTransferEventArgs e)
        {
            FrameworkElement src = e.Source as FrameworkElement;
            if (src == null)
                return;
            FilterTree tree = src.DataContext as FilterTree;
            if (tree == null)
                return;
            MarkNodeAsUpdated((FilterTree)tree.Children[0]);
        }

        private void ValueUpdated(object sender, DataTransferEventArgs e)
        {
            FrameworkElement src = e.Source as FrameworkElement;
            if (src == null)
                return;
            FilterTree tree = src.DataContext as FilterTree;
            if (tree == null)
                return;
            MarkNodeAsUpdated((FilterTree)tree.Children[0].GetChild(1));
        }

        private void AddNodeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SQLTree tree = e.Parameter as SQLTree;
            if (tree != null)
                AddNode(tree);
        }

        private void AddGroupExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SQLTree tree = e.Parameter as SQLTree;
            if (tree != null)
                AddGroupNode(tree);
        }

        private void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SQLTree tree = e.Parameter as SQLTree;
            if (tree != null)
                DeleteNode(tree);
        }

        private void DeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            FilterTree tree = e.Parameter as FilterTree;
            e.CanExecute = (tree != null && tree.Parent != null);
        }

        // HACK ALERT: We exploit the point when value in datacontext have been changed but UI did not update yet
        private void StringOperatorUpdated(object sender, DataTransferEventArgs e)
        {
            DropDownBox src = e.Source as DropDownBox;
            if (src == null)
                return;
            FilterTree tree = src.DataContext as FilterTree;
            if (tree == null)
                return;
            int newType = tree.Children[0].Type;
            int oldType = StringExprTypePicker.ConvertBack(src.SelectedIndex);

            if (newType != StringPatternLexer.COMPLEX && oldType == StringPatternLexer.COMPLEX)
            {
                StringTree constTree = tree.Children[0].GetChild(1) as StringTree;
                if (constTree != null)
                {
                    constTree.Text = Filter.Escape(constTree.Text);
                    MarkNodeAsUpdated(constTree);
                }
            }
            else if (newType == StringPatternLexer.COMPLEX && oldType != StringPatternLexer.COMPLEX)
            {
                StringTree constTree = tree.Children[0].GetChild(1) as StringTree;
                if (constTree != null)
                {
                    switch (oldType)
                    {
                        case StringPatternLexer.IS:
                            constTree.Text = "%" + constTree.Text + "%";
                            break;
                        case StringPatternLexer.BEGINS:
                            constTree.Text = constTree.Text + "%";
                            break;
                        case StringPatternLexer.ENDS:
                            constTree.Text = "%" + constTree.Text;
                            break;
                    }
                }
                MarkNodeAsUpdated(constTree);
            }
            MarkNodeAsUpdated((FilterTree)tree.Children[0]);
        }

        internal void UpdateFilter(string arg)
        {
            if (arg != "Function")
                return;
            ICollectionView view = CollectionViewSource.GetDefaultView(ServerList.ItemsSource);
            view.Filter = (o) => FilterObject.Function((FakeServer)o);
        }

    }
}
