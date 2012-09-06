using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vosen.SQLFilter;
using System.Net;

namespace Sonar.Tests.SQLFilter
{
    [Flags]
    public enum ServerSubtype : byte
    {
        None,
        Listen,
        Dedicated,
        TV,
    }

    internal class FakeServer
    {
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public long? Ping { get; set; }

        [Filterable]
        public IPAddress FakeAddress { get; set; }
        // dynamic
        [Filterable]
        public bool? IsLong { get; set; }
        [Filterable]
        public bool? IsSecure { get; set; }
        public long? Latency { get; set; }
        [Filterable]
        public string Map { get; set; }
        [Filterable]
        public int ID { get; set; }
        [Filterable]
        public bool IsCached { get; set; }
        public string GameDir { get; set; }
        public string GameDescription { get; set; }
        [Filterable]
        public int? Players { get; set; }
        public int? FakePlayers { get; set; }
        public int? MaximumPlayers { get; set; }
        [Filterable]
        public ServerSubtype Subtype { get; set; }
        public string Version { get; set; }
        public string TVAddress { get; set; }
        public ICollection<string> Tags { get; set; }

        // the rest
        public string Password { get; set; }
        public string Rcon { get; set; }
    }
}
