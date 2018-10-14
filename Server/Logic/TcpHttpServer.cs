using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace PowerSocketServer.Logic
{
    class TcpHttpServer
    {
        private string _address;
        private int _port;

        public string GetAddress()
        {
            return _address;
        }

        public int GetPort()
        {
            return _port;
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
            _port = ((IPEndPoint)listener.LocalEndpoint).Port;
            _address = string.Format("http://{0}:{1}/", IPAddress.Loopback, _port);

            

            bool run = true;

            while (run)
            {
                var client = await listener.AcceptTcpClientAsync();

                // Parse HTTP header
                var response = ReadString(client);

                string path = null;
                string content = null;
                byte[] bytes = null;

                // Parse HTTP header - get the first line
                string first = new StringReader(response).ReadLine();
                if (first != null)
                {
                    string[] res = first.Split(' ');
                    if (res.Length >= 2)
                    {
                        string method = res[0];
                        path = res[1];


                        // / --> /remote
                        // /slides (WARNING: insecure)
                        // /remote
                        // /stage


                        if (path.StartsWith("/slides/"))
                        {
                            path = path.Replace("/slides/", "");
                            try
                            {
                                bytes = File.ReadAllBytes(
                                    System.IO.Path.Combine(Helpers.TempDir.GetTempDirPath(), path));
                            }
                            catch (Exception e)
                            {
                                content = "404";
                            }
                            
                        }
                        else
                        {

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
                                content = Helpers.EmbeddedResource.GetResource("PowerSocketServer.Public." + path);
                            }
                        }

                    }
                }


                WriteStringAsync(client, content, bytes, path).ContinueWith(t =>
                {
                    // callback
                    client.Dispose();
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

        private Task WriteStringAsync(TcpClient client, string str, byte[] bytes, string path)
        {
            return Task.Run(() =>
            {
                using (var writer = new StreamWriter(client.GetStream(), new UTF8Encoding(false)))
                {
                    if (str == null && bytes == null)
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
                        
                        if (bytes != null)
                        {
                            try
                            {
                                writer.Write("Content-Length: " + bytes.Length);
                                writer.Write(Environment.NewLine);
                                writer.Write(Environment.NewLine);
                                writer.Flush();
                                writer.BaseStream.Write(bytes, 0, bytes.Length);
                            }
                            catch (Exception e)
                            {

                            }
                            
                        }
                        else
                        {
                            try
                            {
                                writer.Write("Content-Length: " + str.Length);
                                writer.Write(Environment.NewLine);
                                writer.Write(Environment.NewLine);
                                writer.Write(str);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            });
        }
    }
}
