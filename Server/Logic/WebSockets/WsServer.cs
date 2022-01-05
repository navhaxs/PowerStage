namespace PowerSocketServer.Logic.WebSockets
{
    using GalaSoft.MvvmLight.Messaging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PowerSocketServer.Models;
    using System.Linq;
    using Unosquare.Labs.EmbedIO;
    using Unosquare.Labs.EmbedIO.Modules;
    using Unosquare.Swan;

    /// <inheritdoc />
    /// <summary>
    /// Defines a very simple chat server
    /// </summary>
    [WebSocketHandler("/remote")]
    public class WsServer : WebSocketsServer
    {
        public WsServer()
            : base(true)
        {
            // placeholder

            Messenger.Default.Register<ResponseMessage>(this, (ResponseMessage eventMessages) =>
            {
                string payload = "{\"message\": " + JsonConvert.ToString(eventMessages.WsResponseMessage) + "}";
                Broadcast(payload);
            });

            Messenger.Default.Register<StateUpdateMessage>(this, (StateUpdateMessage stateUpdateMessage) =>
            {
                string payload = "{\"response\":\"statusUpdate\", \"status\":" + JsonConvert.SerializeObject(stateUpdateMessage.state) + "}";
                Broadcast(payload);
            });

            Messenger.Default.Register<BroadcastMessage>(this, (BroadcastMessage broadcastMessage) =>
            {
                string payload = "{\"response\":\"broadcast\", \"message\":" + JsonConvert.SerializeObject(broadcastMessage.message)
                                 + ", \"options\":" + JsonConvert.SerializeObject(broadcastMessage.options) +
                                 "}";
                Broadcast(payload);
            });
        }

        /// <inheritdoc />
        protected override void OnMessageReceived(IWebSocketContext context, byte[] rxBuffer,
            IWebSocketReceiveResult rxResult)
        {

            // parse message
            JObject jsonData = JObject.Parse(rxBuffer.ToText());

            switch (jsonData["action"].ToString())
            {
                case Message.AUTHENTICATE:
                    Send(context, "{\"action\":\"authenticate\", \"authenticated\": 1}");
                    break;
                case Message.NEXT_SLIDE:
                    Send(context, "{\"action\":\"ok\"}");
                    Main.api.NextSlide();
                    break;
                case Message.PREV_SLIDE:
                    Send(context, "{\"action\":\"ok\"}");
                    Main.api.PrevSlide();
                    break;
                case Message.SYNC:
                    Send(context, "{\"action\":\"ok\"}");
                    Main.api.SyncState();
                    break;
                case Message.BROADCAST:
                    Send(context, "{\"action\":\"ok\"}");

                    Messenger.Default.Send(new BroadcastMessage()
                    {
                        message = jsonData["message"].ToString(),
                        //options = jsonData["options"]
                    });
                    break;
                default:
                    break;

            }

            //// send to all connected clients
            //foreach (var ws in WebSockets.Where(ws => ws != context))
            //{
            //    Send(ws, rxBuffer.ToText());
            //}
        }


        /// <inheritdoc />
        public override string ServerName => nameof(WsServer);

        /// <inheritdoc />
        protected override void OnClientConnected(
            IWebSocketContext context,
            System.Net.IPEndPoint localEndPoint,
            System.Net.IPEndPoint remoteEndPoint)
        {
            //Send(context, "Welcome to the chat room!");

            //foreach (var ws in WebSockets.Where(ws => ws != context))
            //{
            //    Send(ws, "Someone joined the chat room.")V;
            //}
        }

        /// <inheritdoc />
        protected override void OnFrameReceived(IWebSocketContext context, byte[] rxBuffer,
            IWebSocketReceiveResult rxResult)
        {
            // placeholder
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(IWebSocketContext context)
        {
            //Broadcast("Someone left the chat room.");
        }
    }
}
