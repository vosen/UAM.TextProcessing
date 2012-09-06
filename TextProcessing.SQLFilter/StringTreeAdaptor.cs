using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;

namespace Vosen.SQLFilter
{
    public class StringTreeAdaptor : CommonTreeAdaptor
    {
        public override object Create(Antlr.Runtime.IToken payload)
        {
            return new StringTree(payload);
        }
    }
}
