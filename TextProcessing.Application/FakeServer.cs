using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vosen.SQLFilter;
using System.Net;

namespace SQLFilter.FilterView.Test
{
    public class FakeServer
    {
        [Filterable]
        public string Owner { get; set; }
        [Filterable]
        public string City { get; set; }
        [Filterable]
        public IPAddress Address { get; set; }
        [Filterable]
        public OperatingSystem OS { get; set; }
        [Filterable]
        public int Users { get; set; }
        [Filterable]
        public bool Active { get; set; }

        public FakeServer() { }
    }
}
