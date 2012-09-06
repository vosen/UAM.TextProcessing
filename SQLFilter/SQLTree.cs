using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using Antlr.Runtime;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Net;

namespace Vosen.SQLFilter
{
    //TODO: Do something with NOT LIKE expressions
    public class SQLTree : FilterTree
    {
        public SQLTree()
            : base()
        { }

        public SQLTree(CommonTree node)
            : base(node)
        { }

        public SQLTree(IToken t)
            : base(t)
        { }

        public SQLTree(FilterTree t)
            : base(t)
        { }

        public SQLTree(int type)
            : base(type)
        { }

        public bool IsGroup
        {
            get
            {
                return (Type == SQLFilterLexer.AND) || (Type == SQLFilterLexer.OR) || (Type == SQLFilterLexer.NOT_OR) || (Type == SQLFilterLexer.NOT_AND);
            }
        }

        public override ITree DupNode()
        {
            return new SQLTree(this);
        }

        public override Expression Compile(ParameterExpression param)
        {
            return Compiler.Compile(this, param);
        }
        
        public override string ToString()
        {
            return ReverseParser.ToString(this);
        }
    }
}
