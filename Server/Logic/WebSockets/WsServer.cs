using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using PowerSocketServer.Models;
using System;
using System.Collections.Generic;
//using Nancy.Json;
using System.Diagnostics;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;

namespace PowerSocketServer.Logic
{
    class WsServer : WebSocketBehavior
    {

        public WsServer()   
        {
            //Messenger.Default.Register<ResponseMessage>(this, (ResponseMessage eventMessages) =>
            //{
            //    string payload = "{\"message:\"" + JsonConvert.ToString(eventMessages.WsResponseMessage) + "}";
            //    Send(payload);
            //});
            
            //Messenger.Default.Register<StateUpdateMessage>(this, (StateUpdateMessage stateUpdateMessage) =>
            //{
            //    string payload = "{\"response\":\"statusUpdate\", \"status\":" + JsonConvert.SerializeObject(stateUpdateMessage.state) + "}";
            //    Send(payload);
            //});

            //Messenger.Default.Register<BroadcastMessage>(this, (BroadcastMessage broadcastMessage) =>
            //{
            //    string payload = "{\"response\":\"broadcast\", \"message\":" + JsonConvert.SerializeObject(broadcastMessage.message)
            //                     + ", \"options\":" + JsonConvert.SerializeObject(broadcastMessage.options) + 
            //                     "}";
            //    Send(payload);
            //});
        }

        protected override void OnMessage (MessageEventArgs e)
        {
            string response = "{\"error\":\"bad request\"}";

            Debug.Print(e.Data);

            JObject jsonData = JObject.Parse(e.Data);

            switch (jsonData["action"].ToString())
            {
                case Message.AUTHENTICATE:
                    response = "{\"action\":\"authenticate\", \"authenticated\": 1}";
                    break;
                case Message.NEXT_SLIDE:
                    response = "{\"action\":\"ok\"}";
                    Main.api.NextSlide();
                    break;
                case Message.PREV_SLIDE:
                    response = "{\"action\":\"ok\"}";
                    Main.api.PrevSlide();
                    break;
                case Message.SYNC:
                    response = "{\"action\":\"ok\"}";
                    Main.api.SyncState();
                    break; 
                case Message.BROADCAST:
                    response = "{\"action\":\"ok\"}";
                    Messenger.Default.Send(new BroadcastMessage()
                    {
                        message = jsonData["message"].ToString(),
                        //options = jsonData["options"]
                    });
                    break; 
                default:
                    break;
            
            }

            Send(response);
        }

        private string stateToJson()
        {
            var columns = new Dictionary<string, string>
            {
                { "FirstName", "Mathew"},
                { "Surname", "Thompson"},
                { "Gender", "Male"},
                { "SerializeMe", "GoOnThen"}
            };

            //var jsSerializer = new JavaScriptSerializer();


            return null;
//            var serialized = jsSerializer.Serialize(columns);

//            string json = @"{
//  'Name': 'Bad Boys',
//  'ReleaseDate': '1995-4-7T00:00:00',
//  'Genres': [
//    'Action',
//    'Comedy'
//  ]
//}";


//            return "{\"action\":\"authenticate\", \"authenticated\": 1, \"response\":{\"state\":{\"total\":"+Main.api.GetStateInfo().totalSlidesCount+",\"current\":"+Main.api.GetStateInfo().currentSlideIndex+"}}}";
        }
    }
}
