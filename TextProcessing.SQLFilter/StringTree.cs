using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using Antlr.Runtime;
using System.Linq.Expressions;
using System.Reflection;

namespace Vosen.SQLFilter
{
    public class StringTree : FilterTree
    {
        public StringTree()
            : base()
        { }

        public StringTree(CommonTree node)
            : base(node)
        { }

        public StringTree(IToken t)
            : base(t)
        { }

        public StringTree(FilterTree t)
            : base(t)
        { }

        public StringTree(int type)
            : base(type)
        { }

        public override ITree DupNode()
        {
            return new StringTree(this);
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
