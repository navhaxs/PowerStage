using GalaSoft.MvvmLight.Messaging;
using PowerSocketServer;
using PowerSocketServer.Helpers;
using PowerSocketServer.Models;
using PowerSocketServer.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PowerSocketServer.Logic;
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

            
            //System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            Main.wsServer = new WebSocketServer(50002);
            Main.wsServer.AddWebSocketService<PowerSocketServer.Logic.WsServer> ("/remote");
            Main.wsServer.Start();
            WebAddress = string.Join(",", NetworkAddress.GetLocalIPAddress()); //Main.wsServer.Address.ToString();

            // @jeremy TODO this must be run as admin due to Windows http.sys ACL
            // Either replace this with a TcpListener, or launch (only) the SimpleHttpServer with elevation,
            // Or send it over websockets

            Main.httpServer = new SimpleHTTPServer(TempDir.GetTempDirPath(), 50003);
            Main.tcpHttpServer = new TcpHttpServer();
            HttpPort = Main.tcpHttpServer.GetAddress(); //Main.httpServer.Port.ToString() + " " +
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

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(String.Format("{0}", HttpPort));
        }
    }
}
