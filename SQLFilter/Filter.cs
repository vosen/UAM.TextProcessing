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

namespace Vosen.SQLFilter
{
    // This class is not thread safe. at all.
    // TODO: This class needs some type caching using GetTypeFromHandle. Badly
    // TODO: Fix reverse compiling mess. Quickly.
    public static class Filter
    {

        public static string Escape(string input)
        {
            return input.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
        }

        public static string Unescape(string input)
        {
            char[] arr = input.ToCharArray();
            char[] unescaped = new char[arr.Length];
            int j = 0;
            for (int i = 0; i < arr.Length; i++, j++)
            {
                if (arr[i] == '\\' && i < arr.Length - 1)
                {
                    if (arr[i + 1] == '\\')
                    {
                        unescaped[j] = '\\';
                        i++;
                    }
                    else if (arr[i + 1] == '%')
                    {
                        unescaped[j] = '%';
                        i++;
                    }
                    else if (arr[i + 1] == '_')
                    {
                        unescaped[j] = '_';
                        i++;
                    }
                    else
                    {
                        unescaped[j] = arr[++i];
                    }
                }
                else
                {
                    unescaped[j] = arr[i];
                }
            }
            return new string(unescaped, 0, j);
        }

        // We've got string pattern on the input and want matching function
        public static Func<string, bool> CompilePattern(string input)
        {
            string cleaned = PatternToRegexp(input);
            Regex regex = new Regex(cleaned, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return regex.IsMatch;
        }


        public static string PatternToRegexp(string input)
        {
            return "^" + convertPatternToRegExp(Regex.Escape(cleanUnrecognizedEscapeSequences(input)).Replace(@"\\", @"\")) + "$";
        }

        private static string cleanUnrecognizedEscapeSequences(string input)
        {
            char[] inpArray = input.ToCharArray();
            char[] converted = new char[inpArray.Length];
            int j = 0;
            for (int i = 0; i < inpArray.Length; i++, j++)
            {
                if (inpArray[i] == '\\' && i < inpArray.Length - 1)
                {
                    if (inpArray[i + 1] == '\\' || inpArray[i + 1] == '%' || inpArray[i + 1] == '_')
                    {
                        converted[j] = inpArray[i];
                        converted[++j] = inpArray[++i];
                    }
                    else
                    {
                        converted[j] = inpArray[++i];
                    }
                }
                else
                {
                    converted[j] = inpArray[i];
                }
            }
            return new string(converted, 0, j);
        }

        // Function replaces:
        // * \% with %
        // * \_ with _
        // * \\ with \\
        // * \<something> with \<something>
        // * % with .*
        // * _ with .
        private static string convertPatternToRegExp(string input)
        {
            char[] inpArray = input.ToCharArray();
            char[] converted = new char[inpArray.Length << 1];
            int j = 0;
            for (int i = 0; i < inpArray.Length; i++, j++)
            {
                if (inpArray[i] == '\\' && i < inpArray.Length - 1)
                {
                    // i, i+1 are escape seq
                    if (inpArray[i + 1] == '\\')
                    {
                        converted[j] = '\\';
                        converted[++j] = '\\';
                        i++;
                    }
                    else if (inpArray[i + 1] == '%')
                    {
                        converted[j] = '%';
                        i++;
                    }
                    else if (inpArray[i + 1] == '_')
                    {
                        converted[j] = '_';
                        i++;
                    }
                    else
                    {
                        converted[j] = '\\';
                        converted[++j] = inpArray[++i];
                    }
                }
                else if (inpArray[i] == '%')
                {
                    converted[j] = '.';
                    converted[++j] = '*';
                }
                else if (inpArray[i] == '_')
                {
                    converted[j] = '.';
                }
                else
                {
                    converted[j] = inpArray[i];
                }
            }
            return new string(converted, 0, j);
        }

        public static void ToEnumExpr(FilterTree tree, Type newType)
        {
            changeCommonLeafNodeType(tree, SQLFilterLexer.ENUM_EXPR, SQLFilterLexer.EQUALS, SQLFilterLexer.STRING, Enum.GetName(newType,0));
        }

        public static void ToNumExpr(FilterTree tree)
        {
            changeCommonLeafNodeType(tree, SQLFilterLexer.NUM_EXPR, SQLFilterLexer.EQUALS, SQLFilterLexer.INT, "0");
        }

        public static void ToBoolExpr(FilterTree tree)
        {
            changeCommonLeafNodeType(tree, SQLFilterLexer.BOOL_EXPR, SQLFilterLexer.IS, SQLFilterLexer.TRUE, "True");
        }

        public static void ToIPExpr(FilterTree tree)
        {
            changeCommonLeafNodeType(tree, SQLFilterLexer.IP_EXPR, SQLFilterLexer.EQUALS, SQLFilterLexer.IPV4, "127.0.0.1");
        }

        public static void ToStringExpr(FilterTree tree)
        {
            if (tree.Type == SQLFilterLexer.STRING_EXPR || !tree.IsLeaf)
                return;
            
            FilterTree op = (FilterTree)tree.Children[0];
            op.Type = StringPatternLexer.IS;
            if (!(op is StringTree))
                tree.SetChild(0, new StringTree(op));
            // do conversions
            FilterTree constant = (FilterTree)tree.Children[0].GetChild(1);
            constant.Type = StringPatternLexer.TEXT;
            constant.Text = "";
            if (!(constant is StringTree))
                tree.Children[0].SetChild(1, new StringTree(constant));
            tree.Type = SQLFilterLexer.STRING_EXPR;
            ((FilterTree)tree.Children[0].GetChild(0)).MarkAsDirty();
        }

        private static void changeCommonLeafNodeType(FilterTree tree, int newType, int newOp, int newConst, string newValue)
        {
            if (tree.Type == newType || !tree.IsLeaf)
                return;            
            FilterTree op = (FilterTree)tree.Children[0];
            op.Type = newOp;
            if (!(op is SQLTree))            
                tree.SetChild(0, new SQLTree(op));            
            // do conversions
            FilterTree constant = (FilterTree)tree.Children[0].GetChild(1);
            constant.Type = newConst;
            constant.Text = newValue;            
            if (!(constant is SQLTree))
                tree.Children[0].SetChild(1, new SQLTree(constant));
            tree.Type = newType;
            ((FilterTree)tree.Children[0].GetChild(0)).MarkAsDirty();
        }
    }
}
