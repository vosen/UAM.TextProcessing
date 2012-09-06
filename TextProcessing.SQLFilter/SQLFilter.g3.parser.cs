using Antlr.Runtime.Tree;
using Antlr.Runtime;

namespace Vosen.SQLFilter
{
    partial class SQLFilterParser
    {
        partial void CreateTreeAdaptor(ref ITreeAdaptor adaptor)
        {
            adaptor = new SQLTreeAdaptor();
        }


        private FilterTree stringPatternAST(string source)
        {
            try
            {
                ANTLRStringStream input = new ANTLRStringStream(source);
                StringPatternLexer lexer = new StringPatternLexer(input);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                StringPatternParser parser = new StringPatternParser(tokens);
                return parser.parse().Tree;
            }
            catch { }
            return null;
        }

        private string stripQuotes(string source)
        {
            return source.Substring(1, source.Length - 2);
        }

        private FilterTree makeFirstLeftSubchild(FilterTree root, ITree child)
        {
            root.Children.Insert(0, child);
            root.FreshenParentAndChildIndexes();
            return root;
        }

        private FilterTree rewriteWithNot(FilterTree tree)
        {
            switch(tree.Type)
            {
                case SQLFilterLexer.AND:
                    tree.Token.Type = SQLFilterLexer.NOT_AND;
                    return tree;
                case SQLFilterLexer.OR:
                    tree.Token.Type = SQLFilterLexer.NOT_OR;
                    return tree;
            }
            return (FilterTree)adaptor.BecomeRoot(new CommonToken(SQLFilterLexer.NOT), tree);
        }

        private FilterTree reverseOperator(FilterTree tree)
        {
            switch(tree.Type)
            {
                case SQLFilterParser.GREATER:
                    tree.Token.Type = SQLFilterLexer.LESSER;
                    break;
                case SQLFilterParser.GREATEROREQUALS:
                    tree.Token.Type = SQLFilterParser.LESSEROREQUALS;
                    break;
                case SQLFilterParser.LESSER:
                    tree.Token.Type = SQLFilterParser.GREATER;
                    break;
                case SQLFilterParser.LESSEROREQUALS:
                    tree.Token.Type = SQLFilterParser.GREATEROREQUALS;
                    break;
            }
            return tree;
        }

        // Mark single expressions as leafs
        private static void markAsLeaf(FilterTree tree)
        {
            tree.IsLeaf = true;
        }

        // We want leafs to be grouped inside AND. leaf -> ^(AND leaf)
        private FilterTree groupify(FilterTree tree)
        {
            if (!isGroupNode(tree))
                return (FilterTree)adaptor.BecomeRoot(new CommonToken(SQLFilterLexer.AND), tree);
            return tree;
        }

        // Helper function
        private static bool isGroupNode(FilterTree node)
        {
            return node.Type == SQLFilterLexer.AND || node.Type == SQLFilterLexer.OR || node.Type == SQLFilterLexer.NOT_AND || node.Type == SQLFilterLexer.NOT_OR;
        }
    }
}
