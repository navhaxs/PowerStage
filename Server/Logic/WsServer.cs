using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using PowerSocketServer.Models;
using System;
using System.Diagnostics;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PowerSocketServer.Logic
{
    class WsServer : WebSocketBehavior
    {

        public WsServer()   
        {
            Messenger.Default.Register<EventMessages>(this, (EventMessages eventMessages) =>
            {
                string payload = "{\"message\":" + JsonConvert.ToString(eventMessages.Message) + "}";
                Send(payload);
            });
        }

        protected override void OnMessage (MessageEventArgs e)
        {
            Debug.Print(e.Data);
            switch (e.Data)
            {
                case Message.NEXT_SLIDE:
                    Main.api.NextSlide();
                    break;
                case Message.PREV_SLIDE:
                    Main.api.PrevSlide();
                    break;
                default:
                    break;
            
            }

            Send("{\"action\":\"authenticate\", \"authenticated\": 1}");

        }
    }
}
