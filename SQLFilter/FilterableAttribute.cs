using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vosen.SQLFilter
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Filterable : Attribute
    {
        public Filterable()
        : base()
        {}
    }
}
