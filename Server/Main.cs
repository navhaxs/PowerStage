using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSocketServer
{
    static class Main
    {
        public static Logic.PowerPointApi api = new Logic.PowerPointApi(NetOffice.PowerPointApi.Application.GetActiveInstance());
    }
}
