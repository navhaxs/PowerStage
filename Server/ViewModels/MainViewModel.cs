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

        public string WiFiAddress { get; set; }

        private bool _IsExportingSlides;
        public bool IsExportingSlides
        {
            get
            {
                return _IsExportingSlides;
            }
            set
            {
                _IsExportingSlides = value;
                OnPropertyChanged();
            }
        }

        private double _Progress;
        public double Progress
        {
            get
            {
                return _Progress;
            }
            set
            {
                _Progress = value;
                OnPropertyChanged();
            }
        }

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
