using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using PowerStage.Models;

namespace PowerStage.ViewModels
{
    class StageDisplayWindowViewModel : INotifyPropertyChanged
    {

        private string _statusLabel = "...";
        public string StatusLabel
        {
            get { return _statusLabel; }
            set
            {
                if (value == _statusLabel) return;
                _statusLabel = value;
                OnPropertyChanged("StatusLabel");
            }
        }

        public StageDisplayWindowViewModel()
        {
            
            Messenger.Default.Register<StageMsgMessage>(this,
                (StageMsgMessage o) => {
                    if (o.Visibility)
                    {
                        StageMessageVisibility = Visibility.Visible;
                    }
                    else
                    {
                        StageMessageVisibility = Visibility.Hidden;
                    }
                    StageMessageText = o.Msg;
                }
            );

            Messenger.Default.Register<SlideTextUpdateMessage>(this,
                (SlideTextUpdateMessage o) => {
                    SlideText = o.SlideText;
                }
            );

            Messenger.Default.Register<SlideProgressUpdateMessage>(this,
                (SlideProgressUpdateMessage o) => {
                    CurrentSlideNumber = o.CurrentSlideNum;
                    TotalSlideCount = o.TotalSlideNum;
                }
            );
        }

        Visibility _StageMessageVisibility = Visibility.Hidden;
        public System.Windows.Visibility StageMessageVisibility
        {
            get
            {
                return _StageMessageVisibility;
            }
            set
            {
                _StageMessageVisibility = value;

                //if (value == Visibility.Visible)
                //{
                //    StageMessageBorderColor = Brushes.Red;
                //} else
                //{
                //    StageMessageBorderColor = Brushes.Transparent;
                //}

                OnPropertyChanged("StageMessageVisibility");
            }
        }

        System.Windows.Media.Brush _StageMessageBorderColor;
        public System.Windows.Media.Brush StageMessageBorderColor
        {
            get
            {
                return _StageMessageBorderColor;
            }
            set
            {
                _StageMessageBorderColor = value;
                OnPropertyChanged("StageMessageBorderColor");
            }
        }

        string _StageMessageText;
        public string StageMessageText
        {
            get
            {
                return _StageMessageText;
            }
            set
            {
                _StageMessageText = value;
                OnPropertyChanged("StageMessageText");
            }
        }

        string _SlideText;
        public string SlideText
        {
            get
            {
                return _SlideText;
            }
            set
            {
                _SlideText = value;
                OnPropertyChanged("SlideText");
            }
        }

        int _CurrentSlideNumber;
        public int CurrentSlideNumber
        {
            get
            {
                return _CurrentSlideNumber;
            }
            set
            {
                _CurrentSlideNumber = value;
                OnPropertyChanged("CurrentSlideNumber");
            }
        }

        int _TotalSlideCount;
        public int TotalSlideCount
        {
            get
            {
                return _TotalSlideCount;
            }
            set
            {
                _TotalSlideCount = value;
                OnPropertyChanged("TotalSlideCount");
            }
        }
        public string labelStatus_Text
        {
            get
            {
                return "Wiimote not connected";
            }
        }

        public System.Windows.Visibility Scene_WiimotesActive
        {
            get
            {
                return System.Windows.Visibility.Visible;
            }
        }

        Visibility _Scene_NoBluetooth = Visibility.Visible;
        public System.Windows.Visibility Scene_NoBluetooth
        {
            get
            {
                return _Scene_NoBluetooth;
            }
            set
            {
                _Scene_NoBluetooth = value;
                OnPropertyChanged("Scene_NoBluetooth");
            }
        }

        Visibility _ShowWiimoteHowto = Visibility.Hidden;
        public System.Windows.Visibility ShowWiimoteHowto
        {
            get
            {
                return _ShowWiimoteHowto;
            }
            set
            {
                _ShowWiimoteHowto = value;
                OnPropertyChanged("ShowWiimoteHowto");
            }

        }

        public System.Windows.Visibility ShowIfNoWiimotesActive
        {
            get
            {
                return System.Windows.Visibility.Visible;

            }
        }

        public string StatusImage
        {
            get
            {
                return "Theme/StatusAnnotations_Critical_32xLG_color.png";


            }
        }

        public void UpdateUI()
        {
            OnPropertyChanged("Scene_WiimotesActive");
            OnPropertyChanged("ShowIfNoWiimotesActive");
            OnPropertyChanged("labelStatus_Text");
            OnPropertyChanged("StatusImage");
            OnPropertyChanged("Scene_NoBluetooth");
        }

        public void ShowMessage(string msg)
        {
            MessageBox.Show(msg);
        }

        #region "MVVM PropertyChanged code"

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion


    }
}
