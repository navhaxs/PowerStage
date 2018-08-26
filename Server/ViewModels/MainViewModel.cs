using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSocketServer.ViewModels
{
    class MainViewModel : Base
    { 

        public string HttpPort { get; set; }

        public string WebAddress { get; set; }

        public string WiFiAddress { get; set; }

        private string _DebugOutput;
        public string DebugOutput
        {
            get
            {
                return _DebugOutput;
            }
            set
            {
                _DebugOutput = value;
                OnPropertyChanged();
            }
        }
    }
}
