using PowerSocketServer.Logic;
using PowerSocketServer.Logic.HTTP;
using WebSocketSharp.Server;

namespace PowerSocketServer
{
    static class Main
    {
        public static Logic.PowerPointServer api = new Logic.PowerPointServer();
        public static WebSocketServer wsServer;
        //public static SimpleHTTPServer httpServer;
        public static IAbstractHttpServer httpServer;
    }
}