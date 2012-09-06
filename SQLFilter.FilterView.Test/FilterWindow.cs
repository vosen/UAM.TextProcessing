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
using System.Reflection;
using System.Collections.ObjectModel;

namespace SQLFilter.FilterView.Test
{
    public class FilterWindow<T> : Window
    {
        private Filter<T> filterObject;
        public Filter<T> FilterObject
        {
            get
            {
                return filterObject;
            }
            set
            {
                filterObject = value;
                DataContext = value;
            }
        }
        public IList<string> Properties { get; private set; }
        public Dictionary<string, Tuple<int, int>> PropertiesMap { get { return FilterObject.Properties; } }

        public FilterWindow(Filter<T> filter)
            : base()
        {
            FilterObject = filter;
            Properties = new ObservableCollection<string>(filter.Properties.Keys.OrderBy((s) => s));
        }

        protected void MarkNodeAsUpdated(FilterTree tree)
        {
            tree.MarkAsDirty();
        }

        protected void DeleteNode(SQLTree tree)
        {
            FilterTree parent = tree.Parent as FilterTree;
            if (parent != null)
            {
                parent.DeleteChild(tree.ChildIndex);
                parent.MarkAsDirty();
            }
        }

        public static string[] GenerateEnums(string prop)
        {
            if (prop == null || prop == String.Empty)
                return null;
            return Filter<T>.GenerateEnumNames(prop);
        }

        protected void AddNode(SQLTree tree)
        {
            if (tree.IsGroup)
            {
                tree.AddChild(Filter<T>.CreateDefaultExpression(Properties[0]));
                tree.MarkAsDirty();
            }
        }

        protected void AddGroupNode(SQLTree tree)
        {
            if (tree.IsGroup)
            {
                tree.AddChild(Filter<T>.CreateDefaultGroup());
                tree.MarkAsDirty();
            }
        }

        protected void UpdateExpression(FilterTree expressionTree, string oldProp, string newProp)
        {
            int newPropertyType = PropertiesMap[newProp].Item1;
            if (newPropertyType != SQLFilterLexer.ENUM_EXPR && newPropertyType == expressionTree.Type)
                return;
            switch(newPropertyType)
            {
                case SQLFilterLexer.NUM_EXPR:
                    Filter.ToNumExpr(expressionTree);
                    return;
                case SQLFilterLexer.STRING_EXPR:
                    Filter.ToStringExpr(expressionTree);
                    return;
                case SQLFilterLexer.ENUM_EXPR:
                    Type ofT = typeof(T);
                    var newPropType = ofT.GetProperty(newProp, BindingFlags.Public | BindingFlags.Instance).PropertyType;
                    if(newPropType != ofT.GetProperty(oldProp, BindingFlags.Public | BindingFlags.Instance).PropertyType)
                        Filter.ToEnumExpr(expressionTree, newPropType);
                    return;
                case SQLFilterLexer.IP_EXPR:
                    Filter.ToIPExpr(expressionTree);
                    return;
                case SQLFilterLexer.BOOL_EXPR:
                    Filter.ToBoolExpr(expressionTree);
                    return;
            }
            return;
        }
    }
}
