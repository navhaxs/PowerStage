//using Modules;
using System.Diagnostics;
using System.IO;
using System.Reflection;
//using Swan;
using System;
using System.Threading;
using System.Threading.Tasks;


using Unosquare.Swan;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using System.Collections.Generic;
using PowerSocketServer.Logic.WebSockets;

namespace PowerSocketServer.Logic.HTTP
{
    public class EmbedioWebServer : IAbstractHttpServer
    {
        static int PORT = 8977;
        static CancellationTokenSource ctSource;

        public void Start()
        {
            Run();
        }

        public int getPort() { return PORT; }

        private static async Task Run() {
            var url = $"http://*:{PORT}";

            ctSource = new CancellationTokenSource();
            ctSource.Token.Register(() => "Shutting down".Info(nameof(Main)));

            var webOptions = new WebServerOptions(url) { Mode = HttpListenerMode.EmbedIO };

            // Our web server is disposable. 
            using (var server = new WebServer(webOptions))
            {
                // Listen for state changes.
                server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();

                //// First, we will configure our web server by adding Modules.
                //// Please note that order DOES matter.

                // TODO implement sessions for auth
                //// If we want to enable sessions, we simply register the LocalSessionModule
                //// Beware that this is an in-memory session storage mechanism so, avoid storing very large objects.
                //// You can use the server.GetSession() method to get the SessionInfo object and manipulate it.
                //server.RegisterModule(new LocalSessionModule());

                // Register the / route
                server.RegisterModule(new ResourceFilesModule(typeof(PowerSocketServer.Main).Assembly, "PowerSocketServer.Public"));

                // Register /slides/ route
                // TODO: HACK remote / route
                // TODO: Secure file access
                server.RegisterModule(new StaticFilesModule(Helpers.TempDir.GetTempDirPath(), null, new Dictionary<string, string> { { "/slides/", Helpers.TempDir.GetTempDirPath() } }, true));

                //// Register the Web Api Module. See the Setup method to find out how to do it
                //// It registers the WebApiModule and registers the controller(s) -- that's all.
                //server.WithWebApiController<PeopleController>(true);

                // Register the WebSockets module. See the Setup method to find out how to do it
                // It registers the WebSocketsModule and registers the server for the given paths(s)
                server.RegisterModule(new WebSocketsModule());
                server.Module<WebSocketsModule>().RegisterWebSocketsServer<WsServer>();
                //server.Module<WebSocketsModule>().RegisterWebSocketsServer<WebSocketsTerminalServer>();

                server.RegisterModule(new FallbackModule((ctx, ct) => ctx.JsonResponseAsync(new { Message = "Error" }, ct)));

                // Fire up the browser to show the content!
                var browser = new Process
                {
                    StartInfo = new ProcessStartInfo(url.Replace("*", "localhost"))
                    {
                        UseShellExecute = true
                    }
                };

                browser.Start();

                // Once we've registered our modules and configured them, we call the RunAsync() method.
                if (!ctSource.IsCancellationRequested)
                    await server.RunAsync(ctSource.Token);

                // Clean up
            }
        }

        public void Stop()
        {
            
        }
    }
}
