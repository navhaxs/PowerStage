using Newtonsoft.Json.Linq;
using static PowerSocketServer.Logic.PowerPointServer;

namespace PowerSocketServer.Models
{
    // Message to send to client
    class ResponseMessage
    {
        public string WsResponseMessage { get; set; }
    }

    // PowerPoint state update
    class StateUpdateMessage
    {
        public StateInfo state { get; set; }
    }

    class BroadcastMessage
    {
        public string message { get; set; }
        public JToken options { get; set; }
    }

    // Out from powerpointapi
    class SetIsExportingSlides
    {
        public bool IsExportingSlides { get; set; }
        public double Progress { get; set; }
    }

    // In to powerpointapi
    class powerpointApiSyncSlides
    {

    }

}
