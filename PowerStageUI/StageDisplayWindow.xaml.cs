using GalaSoft.MvvmLight.Messaging;
using PowerStage.Models;
using PowerStage.ViewModels;
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
using System.Windows.Shapes;

namespace PowerStage
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class StageDisplayWindow : Window
    {
        public StageDisplayWindow()
        {
            InitializeComponent();

            this.DataContext = new StageDisplayWindowViewModel();

            Messenger.Default.Register<SlideProgressUpdateMessage>(this,
                (SlideProgressUpdateMessage o) => {
                    Dispatcher.Invoke(() =>
                    {
                        if (o.CurrentSlideNum == o.TotalSlideNum)
                        {
                            SlideProgressTextLabel.Content = $"End of slideshow";
                        }
                        else
                        {
                            SlideProgressTextLabel.Content = $"Slide {o.CurrentSlideNum} of {o.TotalSlideNum}";
                        }
                    });

                }
            );
            Messenger.Default.Register<ThumbnailUpdateMessage>(this, HandleTestMessage);
        }

    private void HandleTestMessage(ThumbnailUpdateMessage obj)
    {
        Test.Source = obj.CurrentSlideImage;

        if (obj.NextSlideImageIsEmpty)
        {
            Test1.Source = null;
        }
        else
        {
            Test1.Source = obj.NextSlideImage;
        }

    }

    WindowState last_windowstate;
        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    last_windowstate = this.WindowState;
                    this.WindowState = WindowState.Maximized;
                    this.WindowStyle = WindowStyle.None;
                    this.ResizeMode = ResizeMode.NoResize;
                    break;
                case WindowState.Maximized:
                    if (this.WindowStyle == WindowStyle.None) {
                        // return to windowed mode
                        this.WindowState = last_windowstate;
                        this.WindowStyle = WindowStyle.SingleBorderWindow;
                        this.ResizeMode = ResizeMode.CanResizeWithGrip;
                    } else {
                        // enter fullscreen mode
                        last_windowstate = WindowState.Maximized;
                        this.WindowStyle = WindowStyle.None;
                        this.ResizeMode = ResizeMode.NoResize;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
