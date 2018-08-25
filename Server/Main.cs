using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace PowerSocketServer
{
    static class Main
    {
        public static Logic.PowerPointApi api = new Logic.PowerPointApi(NetOffice.PowerPointApi.Application.GetActiveInstance());
        public static WebSocketServer wsServer;
        public static SimpleHTTPServer httpServer;
        public static TcpHttpServer tcpHttpServer;
    }
}