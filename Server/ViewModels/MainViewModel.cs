using System;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using PowerSocketServer.Models;

namespace PowerSocketServer.ViewModels
{
    class MainViewModel : Base
    { 

        public string HttpPort { get; set; }

        public string WebAddress { get; set; }

        public string IpAddress { get; set; }
        public bool IsExportingSlides { get; set; }

        public double Progress { get; set; }

        public MainViewModel()
        {
            Messenger.Default.Register(this, (SetIsExportingSlides isExportingSlide) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        IsExportingSlides = isExportingSlide.IsExportingSlides;
                        Progress = isExportingSlide.Progress;
                        /* Your code here */
                    }));
                });
        }
    }
}
