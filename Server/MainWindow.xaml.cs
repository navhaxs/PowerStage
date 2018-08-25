using GalaSoft.MvvmLight.Messaging;
using PowerSocketServer;
using PowerSocketServer.Helpers;
using PowerSocketServer.Models;
using System;
using System.Windows;
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

            Main.httpServer = new SimpleHTTPServer(@"C:\Users\Jeremy\Desktop\", 50003); // PORT is ignored
            HttpPort = Main.httpServer.Port.ToString();

            WiFiAddress = string.Join(",", GetNetworkAddress.Fetch().ToArray());
        }

        public String HttpPort { get; set; }

        public String WebAddress { get; set; }

        public String WiFiAddress { get; set; }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Main.wsServer != null)
                Main.wsServer.Stop();

            if (Main.httpServer != null)
                Main.httpServer.Stop();

        }
    }
}
