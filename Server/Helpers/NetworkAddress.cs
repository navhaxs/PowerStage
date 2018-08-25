using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PowerSocketServer.Helpers
{
    static class NetworkAddress
    {
        public static List<string> GetLocalIPAddress()
        {
            List<string> result = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                // todo: add network interface name (.e.g Wi-Fi)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    result.Add(ip.ToString());
                }
            }

            return result;
        }
    }
}
