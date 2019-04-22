using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSocketServer.Utils
{
    static class WebServerUtils
    {
        public static void parseQueryParameters(string uri)
        {
            int start = uri.IndexOf('?');
            if (start >= 0)
            {
                // return a Map of query parameters
            }
        }
    }
}
