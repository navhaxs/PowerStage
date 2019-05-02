using PowerSocketServer;
using PowerSocketServer.Helpers;
using System;
using System.Windows;
using PowerSocketServer.Logic;
using WebSocketSharp.Server;
using PowerSocketServer.Models;
using GalaSoft.MvvmLight.Messaging;
using PowerSocketServer.ViewModels;
using PowerSocketServer.Logic.HTTP;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainViewModel mainViewModel = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mainViewModel;

            
            //System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            //Main.wsServer = new WebSocketServer(50003);
            //Main.wsServer.AddWebSocketService<PowerSocketServer.Logic.WsServerOld> ("/remote");
            //Main.wsServer.Start();
            mainViewModel.WebAddress = string.Join(",", NetworkAddress.GetLocalIPAddress()); //Main.wsServer.Address.ToString();

            // @jeremy TODO this must be run as admin due to Windows http.sys ACL
            // Either replace this with a TcpListener, or launch (only) the SimpleHttpServer with elevation,
            // Or send it over websockets


            // XXXXXXXX jeremy
            //Main.httpServer = new SimpleHTTPServer(TempDir.GetTempDirPath(), 50003);
            Main.httpServer = new EmbedioWebServer();
            Main.httpServer.Start();
            //mainViewModel.HttpPort = $"{Main.httpServer.GetAddress()}"; //Main.httpServer.Port.ToString() + " " +

            var ipAddressArray = GetNetworkAddress.Fetch().ToArray();
            var firstIpAddress = ipAddressArray[0];
            mainViewModel.IpAddress = $"http://{firstIpAddress}:{Main.httpServer.getPort()}";

            // UI Events
            Messenger.Default.Register<StateUpdateMessage>(this, (StateUpdateMessage stateUpdateMessage) =>
            {
                //mainViewModel.DebugOutput = stateUpdateMessage.state.ToString();
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Main.wsServer != null)
                Main.wsServer.Stop();

            if (Main.httpServer != null)
                Main.httpServer.Stop();

        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(String.Format("{0}", mainViewModel.IpAddress));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Topmost = false;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new powerpointApiSyncSlides());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new powerpointApiSync());
        }
    }
}
