using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerSocketServer.Logic
{
    class TcpHttpServer
    {
        private string _address;

        public string GetAddress()
        {
            return _address;
        }

        public TcpHttpServer()
        {
            startAsync();
        }

        public async Task startAsync()
        {

            // Creates a redirect URI using an available port on the loopback address.
            var listener = new TcpListener(IPAddress.Parse("0.0.0.0"), 0);
            listener.Start();
            _address = string.Format("http://{0}:{1}/", IPAddress.Loopback, ((IPEndPoint)listener.LocalEndpoint).Port);

            string path = null;
            // Creates the OAuth 2.0 authorization request.
            //string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
            //    authorizationEndpoint,
            //    System.Uri.EscapeDataString(redirectURI),
            //    clientID,
            //    state,
            //    code_challenge,
            //    code_challenge_method);

            //// Opens request in the browser.
            //System.Diagnostics.Process.Start(authorizationRequest);

            bool run = true;
            int count = 0;
            while (run)
            {
                // Waits for the OAuth authorization response.
                var client = await listener.AcceptTcpClientAsync();
                

                // Read response.
                var response = ReadString(client);

                string text = null;
                // Brings this app back to the foreground.
                //this.Activate();

                string first = new StringReader(response).ReadLine();

                if (first != null)
                {


                    string[] res = first.Split(' ');
                    if (res.Length >= 2)
                    {
                        string method = res[0];
                        path = res[1];

                        var m = path.LastIndexOf('/');
                        if (m > -1)
                        {
                            path = path.Substring(m + 1);
                        }

                        if (path == "")
                        {
                            path = "index.html";
                        }

                        System.Diagnostics.Debug.Print(path);
                        if (Helpers.EmbeddedResource.HasResource("PowerSocketServer.Public." + path))
                        {
                            text = Helpers.EmbeddedResource.GetResource("PowerSocketServer.Public." + path);

                        }

                    }
                }


                    WriteStringAsync(client, text, path).ContinueWith(t => // "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please close this window and return to the app." + count + "</body></html>").ContinueWith(t =>
                {
                    client.Dispose();
                    //listener.Stop();
                    count++;
                    //Console.WriteLine("HTTP server stopped.");
                });


            }

        }

        private string ReadString(TcpClient client)
        {
            var readBuffer = new byte[client.ReceiveBufferSize];
            string fullServerReply = null;

            using (var inStream = new MemoryStream())
            {
                var stream = client.GetStream();

            
                int loop = 0;
                // Wait for data to begin coming in for up to 20 secs
                // (Needed when connecting from another machine outside of localhost to this server, e.g. smartphones!)
                while (!stream.DataAvailable && loop < 2000)
                {
                    loop++;
                    Thread.Sleep(10);
                }

                while (stream.DataAvailable)
                {
                    var numberOfBytesRead = stream.Read(readBuffer, 0, readBuffer.Length);
                    if (numberOfBytesRead <= 0)
                        break;

                    inStream.Write(readBuffer, 0, numberOfBytesRead);
                }

                fullServerReply = Encoding.UTF8.GetString(inStream.ToArray());
            }

            return fullServerReply;
        }

        private Task WriteStringAsync(TcpClient client, string str, string path)
        {
            return Task.Run(() =>
            {
                using (var writer = new StreamWriter(client.GetStream(), new UTF8Encoding(false)))
                {
                    if (str == null)
                    {
                        writer.Write("HTTP/1.0 404 Not Found");
                        writer.Write(Environment.NewLine);
                        writer.Write(Environment.NewLine);
                    }
                    else { 
                        writer.Write("HTTP/1.0 200 OK");
                        writer.Write(Environment.NewLine);

                        //Adding permanent http response headers
                        string mime;
                        writer.Write("ContentType: ", Helpers.HttpServerHelper._mimeTypeMappings.TryGetValue(Path.GetExtension(path), out mime) ? mime : "application/octet-stream");
                        //writer.Write(Environment.NewLine);
                        writer.Write("Date", DateTime.Now.ToString("r"));
                        writer.Write(Environment.NewLine);
                        //writer.Write("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));
                        //writer.Write(Environment.NewLine);

                        //writer.Write("Content-Type: text/html; charset=UTF-8");
                        //writer.Write(Environment.NewLine);
                        writer.Write("Content-Length: " + str.Length);
                        writer.Write(Environment.NewLine);
                        writer.Write(Environment.NewLine);
                        writer.Write(str);
                    }
                }
            });
        }
    }
}
