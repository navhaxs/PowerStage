using static PowerSocketServer.Logic.PowerPointApi;

namespace PowerSocketServer.Models
{

    // Message to send to client
    class ResponseMessage
    {
        public string WsResponseMessage { get; set; }
    }

    class StateUpdateMessage
    {
        public StateInfo state { get; set; }
    }
}
