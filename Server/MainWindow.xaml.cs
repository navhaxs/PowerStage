using PowerSocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketSharp.Server;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Main.wsServer = new WebSocketServer(50002);
            Main.wsServer.AddWebSocketService<PowerSocketServer.Logic.WsServer> ("/remote");
            Main.wsServer.Start();
            WebAddress = Main.wsServer.Address.ToString();

            Main.httpServer = new SimpleHTTPServer(@"C:\Users\Jeremy\Desktop\", 50003);
            HttpPort = Main.httpServer.Port.ToString();
        }

        public String HttpPort { get; set; }

        public String WebAddress { get; set; }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Main.wsServer != null)
                Main.wsServer.Stop();

            if (Main.httpServer != null)
                Main.httpServer.Stop();

        }
    }
}
