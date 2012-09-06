using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vosen.SQLFilter
{
    internal static class ReverseParser
    {
        public static string ToString(SQLTree tree)
        {
            switch (tree.Type)
            {
                case SQLFilterLexer.STRING:
                case SQLFilterLexer.ID:
                case SQLFilterLexer.INT:
                case SQLFilterLexer.IPV4:
                    return tree.Text;
                case SQLFilterLexer.TRUE:
                    return "TRUE";
                case SQLFilterLexer.FALSE:
                    return "FALSE";
                case SQLFilterLexer.STRING_EXPR:
                    return toStringStringExpr(tree);
                case SQLFilterLexer.BOOL_EXPR:
                    return toStringBoolExpr((SQLTree)tree.Children[0]);
                case SQLFilterLexer.ENUM_EXPR:
                    return toStringEnumExpr((SQLTree)tree.Children[0]);
                case SQLFilterLexer.IP_EXPR:
                case SQLFilterLexer.NUM_EXPR:
                    return toStringNumExpr((SQLTree)tree.Children[0]);                
                case SQLFilterLexer.NOT_AND:
                    return String.Format("NOT {0}", toStringAndGroup(tree.Children));
                case SQLFilterLexer.NOT_OR:
                    return String.Format("NOT {0}", toStringOrGroup(tree.Children));
                case SQLFilterLexer.AND:
                    return toStringAndGroup(tree.Children);
                case SQLFilterLexer.OR:
                    return toStringOrGroup(tree.Children);
            }
            return String.Empty;
        }

        private static string toStringOrGroup(IList<Antlr.Runtime.Tree.ITree> children)
        {
            if (children == null || children.Count == 0)
                return "()";
            if (children.Count == 1)
                return children[0].ToString();
            return String.Format("({0})", String.Join(" OR ", children));
        }

        private static string toStringAndGroup(IList<Antlr.Runtime.Tree.ITree> children)
        {
            if (children == null || children.Count == 0)
                return "()";
            if (children.Count == 1)
                return children[0].ToString();
            return String.Format("({0})", String.Join(" AND ", children));
        }

        private static string toStringNumExpr(SQLTree tree)
        {
            switch(tree.Type)
            {
                case SQLFilterLexer.EQUALS:
                    return String.Format("{0} = {1}", tree.Children[0].ToString(), tree.Children[1].ToString());
                case SQLFilterLexer.NOTEQUALS:
                    return String.Format("{0} <> {1}", tree.Children[0].ToString(), tree.Children[1].ToString());
                case SQLFilterLexer.GREATER:
                    return String.Format("{0} > {1}", tree.Children[0].ToString(), tree.Children[1].ToString());
                case SQLFilterLexer.LESSER:
                    return String.Format("{0} < {1}", tree.Children[0].ToString(), tree.Children[1].ToString());
                case SQLFilterLexer.GREATEROREQUALS:
                    return String.Format("{0} >= {1}", tree.Children[0].ToString(), tree.Children[1].ToString());
                case SQLFilterLexer.LESSEROREQUALS:
                    return String.Format("{0} <= {1}", tree.Children[0].ToString(), tree.Children[1].ToString());
            }
            return String.Empty;
        }

        private static string toStringEnumExpr(SQLTree tree)
        {
            switch (tree.Type)
            {
                case SQLFilterLexer.EQUALS:
                    return String.Format(@"{0} = '{1}'", tree.Children[0].ToString(), tree.Children[1].ToString());
                case SQLFilterLexer.NOTEQUALS:
                    return String.Format(@"{0} <> '{1}'", tree.Children[0].ToString(), tree.Children[1].ToString());
            }
            return String.Empty;
        }

        private static string toStringBoolExpr(SQLTree tree)
        {
            return String.Format(@"{0} IS {1}", tree.Children[0].ToString(), tree.Children[1].ToString());
        }

        private static string toStringStringExpr(SQLTree tree)
        {
            if (tree.Children[0].Type == SQLFilterLexer.NOT)
                return ToStringNegate((StringTree)tree.Children[0].GetChild(0));
            return ToString((StringTree)tree.Children[0]);
        }

        public static string ToString(StringTree tree)
        {
            switch (tree.Type)
            {
                // text node
                case StringPatternLexer.TEXT:
                    return tree.Text;
                // Pattern nodes
                case StringPatternLexer.IS:
                    return String.Format("{0} LIKE '{1}'", tree.Children[0].ToString(), tree.Children[1].ToString());
                case StringPatternLexer.CONTAINS:
                    return String.Format("{0} LIKE '%{1}%'", tree.Children[0].ToString(), tree.Children[1].ToString());
                case StringPatternLexer.BEGINS:
                    return String.Format("{0} LIKE '{1}%'", tree.Children[0].ToString(), tree.Children[1].ToString());
                case StringPatternLexer.ENDS:
                    return String.Format("{0} LIKE '%{1}'", tree.Children[0].ToString(), tree.Children[1].ToString());
                case StringPatternLexer.COMPLEX:
                    return String.Format("{0} LIKE '{1}'", tree.Children[0].ToString(), tree.Children[1].ToString());
            }
            return String.Empty;
        }

        private static string ToStringNegate(StringTree tree)
        {
            switch (tree.Type)
            {
                case StringPatternLexer.IS:
                    return String.Format("{0} NOT LIKE '{1}'", tree.Children[0].ToString(), tree.Children[1].ToString());
                case StringPatternLexer.CONTAINS:
                    return String.Format("{0} NOT LIKE '%{1}%'", tree.Children[0].ToString(), tree.Children[1].ToString());
                case StringPatternLexer.BEGINS:
                    return String.Format("{0} NOT LIKE '{1}%'", tree.Children[0].ToString(), tree.Children[1].ToString());
                case StringPatternLexer.ENDS:
                    return String.Format("{0} NOT LIKE '%{1}'", tree.Children[0].ToString(), tree.Children[1].ToString());
                case StringPatternLexer.COMPLEX:
                    return String.Format("{0} NOT LIKE '{1}'", tree.Children[0].ToString(), tree.Children[1].ToString());
            }
            return String.Empty;
        }
    }
}
