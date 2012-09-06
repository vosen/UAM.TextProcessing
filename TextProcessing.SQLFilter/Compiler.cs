using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Antlr.Runtime.Tree;
using System.Numerics;
using System.Reflection;
using System.Net;

namespace Vosen.SQLFilter
{
    internal static class Compiler
    {
        public static Expression Compile(SQLTree node, ParameterExpression param)
        {
            switch (node.Type)
            {
                // Leaf
                case SQLFilterLexer.ID:
                    return compilePropertyNode(node, param);
                // IP expr
                case SQLFilterLexer.IP_EXPR:
                    return compileIP((FilterTree)node.Children[0], param);
                case SQLFilterLexer.IPV4:
                    IPAddress parsed = IPAddress.Parse(node.Text);
                    return Expression.Constant(parsed);
                // Enum expr
                case SQLFilterLexer.ENUM_EXPR:
                    return compileEnum((FilterTree)node.Children[0], param);
                // Bool expr
                case SQLFilterLexer.BOOL_EXPR:
                    return ((FilterTree)node.Children[0]).Compile(param);
                case SQLFilterLexer.IS:
                    if (node.Children[1].Type == SQLFilterLexer.TRUE)
                        return ((FilterTree)node.Children[0]).Compile(param);
                    if (node.Children[1].Type == SQLFilterLexer.FALSE)
                        return Expression.Not(((FilterTree)node.Children[0]).Compile(param));
                    break;
                // Num Expr
                case SQLFilterLexer.NUM_EXPR:
                    return ((FilterTree)node.Children[0]).Compile(param);
                case SQLFilterLexer.INT:
                    return compileNumConstantNode(node);
                case SQLFilterLexer.LESSER:
                    return Expression.LessThan(
                        ((FilterTree)node.Children[0]).Compile(param),
                        ((FilterTree)node.Children[1]).Compile(param));
                case SQLFilterLexer.LESSEROREQUALS:
                    return Expression.LessThanOrEqual(
                        ((FilterTree)node.Children[0]).Compile(param),
                        ((FilterTree)node.Children[1]).Compile(param));
                case SQLFilterLexer.GREATER:
                    return Expression.GreaterThan(
                        ((FilterTree)node.Children[0]).Compile(param),
                        ((FilterTree)node.Children[1]).Compile(param));
                case SQLFilterLexer.GREATEROREQUALS:
                    return Expression.GreaterThanOrEqual(
                        ((FilterTree)node.Children[0]).Compile(param),
                        ((FilterTree)node.Children[1]).Compile(param));
                case SQLFilterLexer.EQUALS:
                    return Expression.Equal(
                        ((FilterTree)node.Children[0]).Compile(param),
                        ((FilterTree)node.Children[1]).Compile(param));
                case SQLFilterLexer.NOTEQUALS:
                    return Expression.NotEqual(
                        ((FilterTree)node.Children[0]).Compile(param),
                        ((FilterTree)node.Children[1]).Compile(param));
                // String Expr
                case SQLFilterLexer.STRING_EXPR:
                    if (node.Children[0] is SQLTree && node.Children[0].Type == SQLFilterLexer.NOT)
                        return Expression.Not(((StringTree)node.Children[0].GetChild(0)).Compile(param));
                    else
                        return ((StringTree)node.Children[0]).Compile(param);
                // Expr groups
                case SQLFilterLexer.AND:
                    return compileAnd(node.Children, param);
                case SQLFilterLexer.OR:
                    return compileOr(node.Children, param);
                case SQLFilterLexer.NOT_AND:
                    return Expression.Not(compileAnd(node.Children, param));
                case SQLFilterLexer.NOT_OR:
                    return Expression.Not(compileOr(node.Children, param));
            }
            return null;
        }

        public static Expression Compile(StringTree node, ParameterExpression param)
        {
            switch (node.Type)
            {
                // text node
                case StringPatternLexer.TEXT:
                    return Expression.Constant(node.Text);
                // Pattern nodes
                case StringPatternLexer.IS:
                    return Expression.Call(
                        typeof(StringExtension).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(string), typeof(StringComparison) }, null),
                        new Expression[] { ((FilterTree)node.Children[0]).Compile(param), ((FilterTree)node.Children[1]).Compile(param), Expression.Constant(StringComparison.OrdinalIgnoreCase) });
                case StringPatternLexer.CONTAINS:
                    return Expression.Call(
                        typeof(StringExtension).GetMethod("Contains", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(string), typeof(StringComparison) }, null),
                        new Expression[] { ((FilterTree)node.Children[0]).Compile(param), ((FilterTree)node.Children[1]).Compile(param), Expression.Constant(StringComparison.OrdinalIgnoreCase) });
                case StringPatternLexer.BEGINS:
                    return Expression.Call(
                        typeof(StringExtension).GetMethod("StartsWith", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(string), typeof(StringComparison) }, null),
                        new Expression[] { ((FilterTree)node.Children[0]).Compile(param), ((FilterTree)node.Children[1]).Compile(param), Expression.Constant(StringComparison.OrdinalIgnoreCase) });
                case StringPatternLexer.ENDS:
                    return Expression.Call(
                        typeof(StringExtension).GetMethod("EndsWith", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(string), typeof(StringComparison) }, null),
                        new Expression[] { ((FilterTree)node.Children[0]).Compile(param), ((FilterTree)node.Children[1]).Compile(param), Expression.Constant(StringComparison.OrdinalIgnoreCase) });
                case StringPatternLexer.COMPLEX:
                    {
                        Func<string, bool> validator = Filter.CompilePattern(node.Children[1].Text);
                        Expression<Func<string, bool>> lambda = (s) => (s == null) ? false : validator(s);
                        return Expression.Invoke(
                            lambda,
                            new Expression[] { ((FilterTree)node.Children[0]).Compile(param) });
                    }
            }
            return null;
        }

        private static Expression compileIP(FilterTree node, ParameterExpression param)
        {
            switch (node.Type)
            {
                case SQLFilterLexer.LESSER:
                    return Expression.Call(
                        typeof(IPExtension).GetMethod("LessThan", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(IPAddress), typeof(IPAddress) }, null),
                        new Expression[] { ((FilterTree)node.Children[0]).Compile(param), ((FilterTree)node.Children[1]).Compile(param) });
                case SQLFilterLexer.LESSEROREQUALS:
                    return Expression.Call(
                        typeof(IPExtension).GetMethod("LessThanOrEqual", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(IPAddress), typeof(IPAddress) }, null),
                        new Expression[] { ((FilterTree)node.Children[0]).Compile(param), ((FilterTree)node.Children[1]).Compile(param) });
                case SQLFilterLexer.GREATER:
                    return Expression.Call(
                        typeof(IPExtension).GetMethod("GreaterThan", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(IPAddress), typeof(IPAddress) }, null),
                        new Expression[] { ((FilterTree)node.Children[0]).Compile(param), ((FilterTree)node.Children[1]).Compile(param) });
                case SQLFilterLexer.GREATEROREQUALS:
                    return Expression.Call(
                        typeof(IPExtension).GetMethod("GreaterThanOrEqual", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(IPAddress), typeof(IPAddress) }, null),
                        new Expression[] { ((FilterTree)node.Children[0]).Compile(param), ((FilterTree)node.Children[1]).Compile(param) });
                case SQLFilterLexer.EQUALS:
                    return Expression.Call(
                        ((FilterTree)node.Children[0]).Compile(param),
                        typeof(IPAddress).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(object) }, null),
                        new Expression[] { ((FilterTree)node.Children[1]).Compile(param) });
                case SQLFilterLexer.NOTEQUALS:
                    return Expression.Not(
                        Expression.Call(
                            ((FilterTree)node.Children[0]).Compile(param),
                            typeof(IPAddress).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(object) }, null),
                            new Expression[] { ((FilterTree)node.Children[1]).Compile(param) }));
            }
            return null;
        }

        private static Expression compileEnum(FilterTree node, ParameterExpression param)
        {
            Expression property = ((FilterTree)node.Children[0]).Compile(param);
            string enumText = node.Children[1].Text;
            object parsed;
            try
            {
                parsed = Enum.Parse(property.Type, enumText, false);
            }
            catch
            {
                // HACK ALERT: It's kinda late for fixing the tree but whatever
                ((FilterTree)node.Children[1]).Text = Enum.GetNames(property.Type)[0];
                parsed = Enum.GetValues(property.Type).GetValue(0);
            }

            switch (node.Type)
            {
                case SQLFilterLexer.EQUALS:
                    return Expression.Equal(
                        property,
                        Expression.Constant(parsed, property.Type));
                case SQLFilterLexer.NOTEQUALS:
                    return Expression.NotEqual(
                        property,
                        Expression.Constant(parsed, property.Type));
            }
            return null;
        }

        private static Expression compilePropertyNode(SQLTree node, ParameterExpression param)
        {
            Expression property = Expression.Property(param, node.Text);
            switch (node.SubType)
            {
                case SQLFilterLexer.BOOL:
                    return normalizeNullToFalse(Expression.TypeAs(property, typeof(bool?)));
                case SQLFilterLexer.INT8:
                    return Expression.TypeAs(property, typeof(byte?));
                case SQLFilterLexer.INT16:
                    return Expression.TypeAs(property, typeof(short?));
                case SQLFilterLexer.INT32:
                    return Expression.TypeAs(property, typeof(int?));
                case SQLFilterLexer.INT64:
                    return Expression.TypeAs(property, typeof(long?));
            }
            return property;
        }

        private static Expression compileNumConstantNode(SQLTree node)
        {
            switch (node.SubType)
            {
                case SQLFilterLexer.INT8:
                    {
                        BigInteger value = BigInteger.Parse(node.Text);
                        if (value <= Byte.MinValue)
                            return Expression.Constant(Byte.MinValue, typeof(byte?));
                        if (value >= Byte.MaxValue)
                            return Expression.Constant(Byte.MaxValue, typeof(byte?));
                        return Expression.Constant((byte?)value, typeof(byte?));
                    }
                case SQLFilterLexer.INT16:
                    {
                        BigInteger value = BigInteger.Parse(node.Text);
                        if (value <= Int16.MinValue)
                            return Expression.Constant(Int16.MinValue, typeof(short?));
                        if (value >= Int16.MaxValue)
                            return Expression.Constant(Int16.MaxValue, typeof(short?));
                        return Expression.Constant((short?)value, typeof(short?));
                    }
                case SQLFilterLexer.INT32:
                    {
                        BigInteger value = BigInteger.Parse(node.Text);
                        if (value <= Int32.MinValue)
                            return Expression.Constant(Int32.MinValue, typeof(int?));
                        if (value >= Int32.MaxValue)
                            return Expression.Constant(Int32.MaxValue, typeof(int?));
                        return Expression.Constant((int?)value, typeof(int?));
                    }
                case SQLFilterLexer.INT64:
                    {
                        BigInteger value = BigInteger.Parse(node.Text);
                        if (value <= Int64.MinValue)
                            return Expression.Constant(Int64.MinValue, typeof(long?));
                        if (value >= Int64.MaxValue)
                            return Expression.Constant(Int64.MaxValue, typeof(long?));
                        return Expression.Constant((long?)value, typeof(long?));
                    }
            }
            return Expression.Constant(BigInteger.Parse(node.Text), typeof(BigInteger?));
        }

        // That's terrible function signature but we avoid copying everything
        private static Expression compileOr(IList<ITree> children, ParameterExpression param)
        {
            if (children == null)
                return null;

            var compiledChildren = children.Cast<FilterTree>().Select((n) => n.Compile(param)).Where((n) => n != null).ToList();
            if (compiledChildren.Count == 0)
                return null;
            if (compiledChildren.Count == 1)
                return compiledChildren[0];
            // Multiple expressions, lovely
            Expression aggr = Expression.OrElse(compiledChildren[0], compiledChildren[1]);
            for (int i = 2; i < compiledChildren.Count; i++)
            {
                aggr = Expression.OrElse(aggr, compiledChildren[i]);
            }
            return aggr;
        }

        private static Expression compileAnd(IList<ITree> children, ParameterExpression param)
        {
            if (children == null)
                return null;

            var compiledChildren = children.Cast<FilterTree>().Select((n) => n.Compile(param)).Where((n) => n != null).ToList();
            if (compiledChildren.Count == 0)
                return null;
            if (compiledChildren.Count == 1)
                return compiledChildren[0];
            // Multiple expressions, lovely
            Expression aggr = Expression.AndAlso(compiledChildren[0], compiledChildren[1]);
            for (int i = 2; i < compiledChildren.Count; i++)
            {
                aggr = Expression.AndAlso(aggr, compiledChildren[i]);
            }
            return aggr;
        }

        private static Expression normalizeNullToFalse(Expression expr)
        {
            return Expression.Coalesce(expr, Expression.Constant(false));
        }

    }
}
