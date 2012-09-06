using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading;
using Antlr.Runtime;
using System.ComponentModel;
using System.Reflection;
using Antlr.Runtime.Tree;
using System.Text.RegularExpressions;
using System.Net;

namespace Vosen.SQLFilter
{
    // This class is not thread safe. at all.
    // TODO: This class needs some type caching using GetTypeFromHandle. Badly
    public class Filter<T> : INotifyPropertyChanged
    {
        public static readonly Dictionary<string, Tuple<int, int>> VisibleProperties;

        public Dictionary<string, Tuple<int, int>> Properties
        {
            get
            {
                return VisibleProperties;
            }
        }

        static Filter()
        {
            VisibleProperties =
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where((p) => Attribute.GetCustomAttribute(p, typeof(Filterable), false) != null)
                    .ToDictionary((a) => a.Name, (a) => pickTypeAndSubtype(a));
        }

        public static bool NormalizeNodes(SQLTree node)
        {
            bool broken = NormalizeNode(node);
            if (node.Children == null)
                return broken;
            for (int i = 0; i < node.Children.Count; i++)
            {
                SQLTree sqlTree = node.Children[i] as SQLTree;
                if (sqlTree != null)
                    if (NormalizeNodes(sqlTree))
                        i--;
            }
            return broken;
        }

        // If node was invalid returns true
        public static bool NormalizeNode(SQLTree node)
        {
            if (!node.IsLeaf)
                return false;

            if (!Filter<T>.VisibleProperties.ContainsKey(node.Children[0].GetChild(0).Text) || node.Type != Filter<T>.VisibleProperties[node.Children[0].GetChild(0).Text].Item1)
            {
                ITree parent = node.Parent;
                parent.DeleteChild(0);
                parent.FreshenParentAndChildIndexes();
                return true;
            }

            if (node.Type == SQLFilterLexer.NUM_EXPR)
            {
                ((FilterTree)node.Children[0].GetChild(1)).SubType = Filter<T>.VisibleProperties[node.Children[0].GetChild(0).Text].Item2;
                ((FilterTree)node.Children[0].GetChild(0)).SubType = Filter<T>.VisibleProperties[node.Children[0].GetChild(0).Text].Item2;
            }
            else if (node.Type == SQLFilterLexer.BOOL_EXPR)
            {
                ((FilterTree)node.Children[0].GetChild(0)).SubType = Filter<T>.VisibleProperties[node.Children[0].GetChild(0).Text].Item2;
            }
            else if (node.Type == SQLFilterLexer.ENUM_EXPR)
            {
                ((FilterTree)node.Children[0].GetChild(1)).SubType = SQLFilterLexer.ENUM;
                ((FilterTree)node.Children[0].GetChild(0)).SubType = -1;
            }
            else
            {
                // cleanup
                foreach (var firstChild in node.Children.Cast<FilterTree>())
                {
                    firstChild.SubType = -1;
                    foreach (var secondChild in firstChild.Children.Cast<FilterTree>())
                    {
                        secondChild.SubType = -1;
                    }
                }
            }

            return false;
        }

        private static Tuple<int, int> pickTypeAndSubtype(PropertyInfo p)
        {
            var propType = p.PropertyType;
            if (propType == typeof(string))
                return new Tuple<int, int>(SQLFilterLexer.STRING_EXPR, -1);
            if (propType == typeof(bool?) || propType == typeof(bool))
                return new Tuple<int, int>(SQLFilterLexer.BOOL_EXPR, SQLFilterLexer.BOOL);
            if (propType == typeof(byte?) || propType == typeof(byte))
                return new Tuple<int, int>(SQLFilterLexer.NUM_EXPR, SQLFilterLexer.INT8);
            if (propType == typeof(short?) || propType == typeof(short))
                return new Tuple<int, int>(SQLFilterLexer.NUM_EXPR, SQLFilterLexer.INT32);
            if (propType == typeof(int?) || propType == typeof(int))
                return new Tuple<int, int>(SQLFilterLexer.NUM_EXPR, SQLFilterLexer.INT32);
            if (propType == typeof(long?) || propType == typeof(long))
                return new Tuple<int, int>(SQLFilterLexer.NUM_EXPR, SQLFilterLexer.INT64);
            if (propType == typeof(IPAddress))
                return new Tuple<int, int>(SQLFilterLexer.IP_EXPR, SQLFilterLexer.IPV4);
            if (propType.IsSubclassOf(typeof(Enum)))
                return new Tuple<int, int>(SQLFilterLexer.ENUM_EXPR, -1);
            return new Tuple<int, int>(0, -1);
        }


        private FilterTree root;
        private string query;
        private Func<T, bool> func;
        private bool treeDirty = true;
        private bool funcDirty = true;
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public FilterTree Tree
        {
            get
            {
                if (treeDirty && query != null)
                    parseQuery(query);
                return root;
            }
        }

        public string Query
        {
            get
            {
                if (root != null && root.IsDirty)
                {
                    query = root.ToString();
                    root.UnmarkWithChildren();
                    NormalizeNodes((SQLTree)root);
                    compileTree();
                }
                return query;
            }
            set
            {
                parseQuery(value);
                /*
                query = value;
                funcDirty = true;
                treeDirty = true;
                OnPropertyChanged("Tree");
                 * */
            }
        }

        public Func<T, bool> Function
        {
            get
            {
                if (funcDirty)
                    compileTree();
                return func;
            }
        }

        public Filter(string whereQuery)
        {
            Query = whereQuery;
        }

        public Filter(string whereQuery, string name)
        {
            Query = whereQuery;
            Name = name;
        }


        private void parseQuery(string newQuery)
        {
            if (root != null)
                root.PropertyChanged -= rootPropertyChanged;
            var stream = new ANTLRStringStream(newQuery);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            try
            {
                root = parser.prog().Tree;
                NormalizeNodes((SQLTree)root);
                compileTree();
                query = root.ToString();
                OnPropertyChanged("Tree");
                OnPropertyChanged("Function");
            }
            catch
            {
                throw;
            }
            finally
            {
                root.PropertyChanged += rootPropertyChanged;
                treeDirty = false;
            }
            OnPropertyChanged("Query");
        }

        private void compileTree()
        {
            ParameterExpression param = Expression.Parameter(typeof(T));
            var body = Tree.Compile(param);
            if (body != null)
            {
                func = Expression.Lambda<Func<T, bool>>(body, new ParameterExpression[] { param }).Compile();
            }
            else
            {
                func = (T p) => true;
            }
            funcDirty = false;
            OnPropertyChanged("Function");
        }

        private static Expression normalizeNullToFalse(Expression expr)
        {
            return Expression.Coalesce(expr, Expression.Constant(false));
        }

        public static string[] GenerateEnumNames(string property)
        {
            if (VisibleProperties[property].Item1 != SQLFilterLexer.ENUM_EXPR)
                return null;
            PropertyInfo propinfo = typeof(T).GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if(propinfo == null)
                return null;
            return Enum.GetNames(propinfo.PropertyType);
        }

        public static FilterTree CreateDefaultExpression(string property)
        {
            var type = VisibleProperties[property];
            switch (type.Item1)
            {
                case SQLFilterLexer.STRING_EXPR:
                    return createDefaultStringExpression(property);
                case SQLFilterLexer.BOOL_EXPR:
                    return createDefaultExpression(property, SQLFilterLexer.BOOL_EXPR, SQLFilterLexer.IS, SQLFilterLexer.TRUE, "True");
                case SQLFilterLexer.NUM_EXPR:
                    return createDefaultExpression(property, SQLFilterLexer.NUM_EXPR, SQLFilterLexer.EQUALS, SQLFilterLexer.INT, "0");
                case SQLFilterLexer.IP_EXPR:
                    return createDefaultExpression(property, SQLFilterLexer.IP_EXPR, SQLFilterLexer.EQUALS, SQLFilterLexer.IPV4, "127.0.0.1");
                case SQLFilterLexer.ENUM_EXPR:
                    Type enumtype = typeof(T).GetProperty(property, BindingFlags.Public | BindingFlags.Instance).PropertyType;
                    return createDefaultExpression(property, SQLFilterLexer.ENUM_EXPR, SQLFilterLexer.EQUALS, SQLFilterLexer.STRING, Enum.GetName(enumtype, 0));
            }
            return null;
        }

        private static FilterTree createDefaultExpression(string property, int exprType, int opType, int constType, string constant)
        {
            SQLTree root = new SQLTree(exprType);
            SQLTree op = new SQLTree(opType);
            SQLTree id = new SQLTree(new CommonToken(SQLFilterLexer.ID, property));
            SQLTree constNode = new SQLTree(new CommonToken(constType, constant));
            op.AddChild(id);
            op.AddChild(constNode);
            root.AddChild(op);
            constNode.MarkAsDirty();
            id.IsDirty = true;
            root.IsLeaf = true;
            return root;
        }

        private static SQLTree createDefaultStringExpression(string property)
        {
            SQLTree root = new SQLTree(SQLFilterLexer.STRING_EXPR);
            StringTree op = new StringTree(StringPatternLexer.IS);
            SQLTree id = new SQLTree(new CommonToken(SQLFilterLexer.ID, property));
            StringTree constant = new StringTree(new CommonToken(StringPatternLexer.TEXT, ""));
            op.AddChild(id);
            op.AddChild(constant);
            root.AddChild(op);
            constant.MarkAsDirty();
            id.IsDirty = true;
            root.IsLeaf = true;
            return root;
        }

        public static FilterTree CreateDefaultGroup()
        {
            return new SQLTree(new CommonToken(SQLFilterLexer.AND));
        }

        public override string ToString()
        {
            return Name;
        }

        # region PropertyChanged

        void rootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
                OnPropertyChanged("Query");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string arg)
        {
            var temp = PropertyChanged;
            if (temp != null)
                temp(this, new PropertyChangedEventArgs(arg));
        }
        #endregion
    }
}
