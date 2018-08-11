using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PowerSocketServer.Logic
{
    class WsServer : WebSocketBehavior
    {
        protected override void OnMessage (MessageEventArgs e)
        {

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

            Send("");
        }
    }
}
