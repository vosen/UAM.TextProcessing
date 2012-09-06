using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

#pragma warning disable 618
namespace Vosen.SQLFilter
{
    internal static class IPExtension
    {
        public static bool GreaterThan(this IPAddress source, IPAddress comparand)
        {
            if (source.AddressFamily != comparand.AddressFamily)
            {
                return false;
            }
            if (source.AddressFamily != AddressFamily.InterNetworkV6)
            {
                return (ulong)IPAddress.HostToNetworkOrder(source.Address) > (ulong)IPAddress.HostToNetworkOrder(comparand.Address);
            }
            var src = source.GetAddressBytes();
            var comp = comparand.GetAddressBytes();
            for (int i = 0; i < 8; i++)
            {
                if (src[i] > comp[i])
                    return true;
                if (src[i] < comp[i])                
                    return false;                
            }
            return false;
        }

        public static bool GreaterThanOrEqual(this IPAddress source, IPAddress comparand)
        {
            if (source.AddressFamily != comparand.AddressFamily)
            {
                return false;
            }
            if (source.AddressFamily != AddressFamily.InterNetworkV6)
            {
                return (ulong)IPAddress.HostToNetworkOrder(source.Address) >= (ulong)IPAddress.HostToNetworkOrder(comparand.Address);
            }
            var src = source.GetAddressBytes();
            var comp = comparand.GetAddressBytes();
            for (int i = 0; i < 8; i++)
            {
                if (src[i] > comp[i])
                    return true;
                if (src[i] < comp[i])
                    return false;
            }
            return true;
        }

        public static bool LessThan(this IPAddress source, IPAddress comparand)
        {
            if (source.AddressFamily != comparand.AddressFamily)
            {
                return false;
            }
            if (source.AddressFamily != AddressFamily.InterNetworkV6)
            {
                return (ulong)IPAddress.HostToNetworkOrder(source.Address) < (ulong)IPAddress.HostToNetworkOrder(comparand.Address);
            }
            var src = source.GetAddressBytes();
            var comp = comparand.GetAddressBytes();
            for (int i = 0; i < 8; i++)
            {
                if (src[i] < comp[i])
                    return true;
                if (src[i] > comp[i])
                    return false;
            }
            return false;
        }

        public static bool LessThanOrEqual(this IPAddress source, IPAddress comparand)
        {
            if (source.AddressFamily != comparand.AddressFamily)
            {
                return false;
            }
            if (source.AddressFamily != AddressFamily.InterNetworkV6)
            {
                return (ulong)IPAddress.HostToNetworkOrder(source.Address) <= (ulong)IPAddress.HostToNetworkOrder(comparand.Address);
            }
            var src = source.GetAddressBytes();
            var comp = comparand.GetAddressBytes();
            for (int i = 0; i < 8; i++)
            {
                if (src[i] < comp[i])
                    return true;
                if (src[i] > comp[i])
                    return false;
            }
            return true;
        }
    }
}
