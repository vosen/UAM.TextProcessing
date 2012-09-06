using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Vosen.SQLFilter;
using Antlr.Runtime.Tree;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Sonar.Tests.SQLFilter
{
    [TestFixture]
    public class Parser
    {
        [Test]
        public void SimpleEnumExpr()
        {
            string input = "\"szafa\" = \'asdas\'";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var val = parser.prog();
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.Type);
            Assert.AreEqual(1, val.Tree.ChildCount);
            ITree exprTree = val.Tree.Children[0];
            Assert.AreEqual(SQLFilterParser.ENUM_EXPR, exprTree.Type);
            Assert.AreEqual(1, exprTree.ChildCount);
            Assert.AreEqual("=", exprTree.GetChild(0).Text);
            Assert.AreEqual(2, exprTree.GetChild(0).ChildCount);
            Assert.AreEqual("szafa", exprTree.GetChild(0).GetChild(0).Text);
            Assert.AreEqual("asdas", exprTree.GetChild(0).GetChild(1).Text);
        }

        [Test]
        public void SimpleStringExpr()
        {
            string input = "\'%%ho sama\' LIKE Map";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var val = parser.prog();
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.Type);
            Assert.AreEqual(1, val.Tree.ChildCount);
            ITree exprTree = val.Tree.Children[0];
            Assert.AreEqual(SQLFilterParser.STRING_EXPR, exprTree.Type);
            Assert.AreEqual(1, exprTree.ChildCount);
            Assert.AreEqual(StringPatternParser.ENDS, exprTree.GetChild(0).Type);
            Assert.AreEqual(2, exprTree.GetChild(0).ChildCount);
            Assert.AreEqual("Map", exprTree.GetChild(0).GetChild(0).Text);
            Assert.AreEqual("ho sama", exprTree.GetChild(0).GetChild(1).Text);
        }

        [Test]
        public void ComplexStringExpr()
        {
            string input = "\'%_%ho _s!ama\' LIKE _karamba";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var val = parser.prog();
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.Type);
            Assert.AreEqual(1, val.Tree.ChildCount);
            ITree exprTree = val.Tree.Children[0];
            Assert.AreEqual(SQLFilterParser.STRING_EXPR, exprTree.Type);
            Assert.AreEqual(1, exprTree.ChildCount);
            Assert.AreEqual(StringPatternParser.COMPLEX, exprTree.GetChild(0).Type);
            Assert.AreEqual(2, exprTree.GetChild(0).ChildCount);
            Assert.AreEqual("_karamba", exprTree.GetChild(0).GetChild(0).Text);
            Assert.AreEqual("%_%ho _s!ama", exprTree.GetChild(0).GetChild(1).Text);
        }

        [Test]
        public void EmptyStringExpr()
        {
            string input = "\'\' LIKE _karamba";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var val = parser.prog();
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.Type);
            Assert.AreEqual(1, val.Tree.ChildCount);
            ITree exprTree = val.Tree.Children[0];
            Assert.AreEqual(SQLFilterParser.STRING_EXPR, exprTree.Type);
            Assert.AreEqual(1, exprTree.ChildCount);
            Assert.AreEqual(StringPatternParser.IS, exprTree.GetChild(0).Type);
            Assert.AreEqual(2, exprTree.GetChild(0).ChildCount);
            Assert.AreEqual("_karamba", exprTree.GetChild(0).GetChild(0).Text);
            Assert.AreEqual("", exprTree.GetChild(0).GetChild(1).Text);
            Assert.AreEqual(StringPatternParser.TEXT, exprTree.GetChild(0).GetChild(1).Type);
        }

        [Test]
        public void SimpleIPExpr()
        {
            string input = "65.43.23.198 > _karamba";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var val = parser.prog();
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.Type);
            Assert.AreEqual(1, val.Tree.ChildCount);
            ITree exprTree = val.Tree.Children[0];
            Assert.AreEqual(SQLFilterParser.IP_EXPR, exprTree.Type);
            Assert.AreEqual(1, exprTree.ChildCount);
            Assert.AreEqual(SQLFilterParser.LESSER, exprTree.GetChild(0).Type);
            Assert.AreEqual(2, exprTree.GetChild(0).ChildCount);
            Assert.AreEqual("_karamba", exprTree.GetChild(0).GetChild(0).Text);
            Assert.AreEqual("65.43.23.198", exprTree.GetChild(0).GetChild(1).Text);
            Assert.AreEqual(SQLFilterParser.IPV4, exprTree.GetChild(0).GetChild(1).Type);
        }

        [Test]
        public void SimpleNumericExpr()
        {
            string input = "6532432 <= balalajka";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var val = parser.prog();
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.Type);
            Assert.AreEqual(1, val.Tree.ChildCount);
            ITree exprTree = val.Tree.Children[0];
            Assert.AreEqual(SQLFilterParser.NUM_EXPR, exprTree.Type);
            Assert.AreEqual(1, exprTree.ChildCount);
            Assert.AreEqual(SQLFilterParser.GREATEROREQUALS, exprTree.GetChild(0).Type);
            Assert.AreEqual(2, exprTree.GetChild(0).ChildCount);
            Assert.AreEqual("balalajka", exprTree.GetChild(0).GetChild(0).Text);
            Assert.AreEqual(SQLFilterParser.ID, exprTree.GetChild(0).GetChild(0).Type);
            Assert.AreEqual("6532432", exprTree.GetChild(0).GetChild(1).Text);
            Assert.AreEqual(SQLFilterParser.INT, exprTree.GetChild(0).GetChild(1).Type);
        }

        [Test]
        public void SimpleExprGroup()
        {
            string input = "6532432 <= balalajka AND _kabza LIKE \'%asfe%\'";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var val = parser.prog();
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.Type);
            Assert.AreEqual(2, val.Tree.ChildCount);
            Assert.AreEqual(SQLFilterParser.NUM_EXPR, val.Tree.GetChild(0).Type);
            Assert.AreEqual(1, val.Tree.GetChild(0).ChildCount);
            Assert.AreEqual(SQLFilterParser.STRING_EXPR, val.Tree.GetChild(1).Type);
            Assert.AreEqual(1, val.Tree.GetChild(1).ChildCount);
        }

        [Test]
        public void NestedExprGroup()
        {
            string input = "\'asdlhad % _\' Like kastro OR _kuskus > 99.32.12.255 OR( 6532432 <= balalajka       AND _kabza LIKE \'%asfe%\' )";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var val = parser.prog();
            /* check expr group */
            Assert.AreEqual(SQLFilterParser.OR, val.Tree.Type);
            Assert.AreEqual(3, val.Tree.ChildCount);
            Assert.AreEqual(SQLFilterParser.STRING_EXPR, val.Tree.GetChild(0).Type);
            Assert.AreEqual(SQLFilterParser.IP_EXPR, val.Tree.GetChild(1).Type);
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.GetChild(2).Type);
            /* check uppermost string expr */
            Assert.AreEqual(2, val.Tree.GetChild(0).GetChild(0).ChildCount);
            Assert.AreEqual(StringPatternLexer.COMPLEX, val.Tree.GetChild(0).GetChild(0).Type);
            Assert.AreEqual("kastro", val.Tree.GetChild(0).GetChild(0).GetChild(0).Text);
            Assert.AreEqual("asdlhad % _", val.Tree.GetChild(0).GetChild(0).GetChild(1).Text);
            /* check uppermost IP expr */
            Assert.AreEqual(2, val.Tree.GetChild(1).GetChild(0).ChildCount);
            Assert.AreEqual(SQLFilterLexer.GREATER, val.Tree.GetChild(1).GetChild(0).Type);
            Assert.AreEqual("_kuskus", val.Tree.GetChild(1).GetChild(0).GetChild(0).Text);
            Assert.AreEqual("99.32.12.255", val.Tree.GetChild(1).GetChild(0).GetChild(1).Text);
            /* check inner int expr */
            Assert.AreEqual(1, val.Tree.GetChild(2).GetChild(0).ChildCount);
            Assert.AreEqual(SQLFilterLexer.NUM_EXPR, val.Tree.GetChild(2).GetChild(0).Type);
            Assert.AreEqual(2, val.Tree.GetChild(2).GetChild(0).GetChild(0).ChildCount);
            Assert.AreEqual(SQLFilterLexer.GREATEROREQUALS, val.Tree.GetChild(2).GetChild(0).GetChild(0).Type);
            /* check inner string expr */
            Assert.AreEqual(1, val.Tree.GetChild(2).GetChild(1).ChildCount);
            Assert.AreEqual(SQLFilterLexer.STRING_EXPR, val.Tree.GetChild(2).GetChild(1).Type);
            Assert.AreEqual(2, val.Tree.GetChild(2).GetChild(1).GetChild(0).ChildCount);
            Assert.AreEqual(StringPatternLexer.CONTAINS, val.Tree.GetChild(2).GetChild(1).GetChild(0).Type);
        }

        [Test]
        public void FreeExprGrouping()
        {
            string input = "a=1 AND b=2 OR c=3 AND d=4 OR e=5";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var val = parser.prog();
            /*outer or tree */
            Assert.AreEqual(SQLFilterParser.OR, val.Tree.Type);
            Assert.AreEqual(3, val.Tree.ChildCount);
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.GetChild(0).Type);
            Assert.AreEqual(SQLFilterParser.AND, val.Tree.GetChild(1).Type);
            Assert.AreEqual(SQLFilterParser.NUM_EXPR, val.Tree.GetChild(2).Type);
        }

        [Test]
        public void NumExprSubtyping()
        {
            string input = @"Players <> 3";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var root = parser.prog().Tree;
            Filter<FakeServer>.NormalizeNodes((SQLTree)root);
            /* now check our tree */
            Assert.AreEqual(SQLFilterLexer.AND, root.Type);
            Assert.AreEqual(SQLFilterLexer.NUM_EXPR, root.Children[0].Type);
            Assert.AreEqual(SQLFilterLexer.INT, root.Children[0].GetChild(0).GetChild(1).Type);
            Assert.AreEqual(SQLFilterLexer.INT32, ((FilterTree)root.Children[0].GetChild(0).GetChild(1)).SubType);
        }

        [Test]
        public void PatternEscaping()
        {
            string input = @"\\\%\_";
            Assert.AreEqual(@"\\\\\\\%\\\_", Filter.Escape(input));
            Assert.AreEqual(input, Filter.Unescape(Filter.Escape(input)));
        }

        [Test]
        public void PatternUnescapingUnknownSeq()
        {
            string input = @"\^";
            Assert.AreEqual(@"^", Filter.Unescape(input));
        }

        [Test]
        public void PatternEscapeUnescapeEmpty()
        {
            string input = @"";
            Assert.AreEqual(input, Filter.Unescape(input));
            Assert.AreEqual(input, Filter.Escape(input));
        }

        [Test]
        public void ConvertPatternToRegexp()
        {
            string input = @"%\%\_\\";
            string outp = Filter.PatternToRegexp(input);
            Assert.AreEqual(@"^.*%_\\$", outp);
        }

        [Test]
        public void ConvertPatternToRegexpSpecialChar()
        {
            string input = @"\^";
            string outp = Filter.PatternToRegexp(input);
            Assert.AreEqual(@"^\^$", outp);
        }


        [Test]
        public void ConvertPatternToRegexpNonSpecialChar()
        {
            string input2 = @"\w";
            string outp2 = Filter.PatternToRegexp(input2);
            Assert.AreEqual(@"^w$", outp2);
        }

        [Test]
        public void NegatedAnd()
        {
            string input = @"NOT (Subtype = 'Listen' AND  Map Like '%ąęw%')";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var root = parser.prog().Tree;
            Filter<FakeServer>.NormalizeNodes((SQLTree)root);
            Assert.AreEqual(SQLFilterLexer.NOT_AND, root.Type);
            Assert.AreEqual(SQLFilterLexer.ENUM_EXPR, root.Children[0].Type);
            Assert.AreEqual(SQLFilterLexer.STRING_EXPR, root.Children[1].Type);
        }

        [Test]
        public void NegatedOr()
        {
            string input = @"NOT (Subtype = 'Listen' OR Map LIKE 'sKazA')";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var root = parser.prog().Tree;
            Filter<FakeServer>.NormalizeNodes((SQLTree)root);
            Assert.AreEqual(SQLFilterLexer.NOT_OR, root.Type);
            Assert.AreEqual(SQLFilterLexer.ENUM_EXPR, root.Children[0].Type);
            Assert.AreEqual(SQLFilterLexer.STRING_EXPR, root.Children[1].Type);
        }

        [Test]
        public void IPV4Expr()
        {
            string input = @"FakeAddress >= 127.0.0.1";
            var stream = new ANTLRStringStream(input);
            var lexer = new SQLFilterLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SQLFilterParser(tokens);
            var root = parser.prog().Tree;
            Filter<FakeServer>.NormalizeNodes((SQLTree)root);
            Assert.AreEqual(SQLFilterLexer.AND, root.Type);
            Assert.AreEqual(SQLFilterLexer.IP_EXPR, root.Children[0].Type);
            Assert.AreEqual(SQLFilterLexer.GREATEROREQUALS, root.Children[0].GetChild(0).Type);
            Assert.AreEqual(SQLFilterLexer.ID, root.Children[0].GetChild(0).GetChild(0).Type);
            Assert.AreEqual(SQLFilterLexer.IPV4, root.Children[0].GetChild(0).GetChild(1).Type);
        }
    }
}
