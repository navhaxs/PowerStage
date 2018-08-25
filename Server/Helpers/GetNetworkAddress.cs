using NetTools;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace PowerSocketServer.Helpers
{
    class GetNetworkAddress
    {
        static string RFC3927_LINK_LOCAL = "169.254.0.0/16";

        public static bool IsLinkLocalAddress(IPAddress ipAddress)
        {
            return IPAddressRange.Parse(RFC3927_LINK_LOCAL).Contains(ipAddress);
        }


    public static List<string> Fetch()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(i => i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) //||i.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                .Where(a => !IsLinkLocalAddress(a.Address))
                .Select(a => a.Address.ToString())
            .ToList();
        }
    }
}
