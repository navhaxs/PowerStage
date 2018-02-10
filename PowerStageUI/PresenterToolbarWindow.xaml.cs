using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PowerStage
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class ToolbarWindow : Window
    {
        public ToolbarWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = SystemParameters.PrimaryScreenHeight * 0.8;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private const int WM_ACTIVATE = 0x0006;
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_ACTIVATE)
            {
                hWnd = lParam;
            }
            return IntPtr.Zero;
        }

        // imports
        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);


        private IntPtr hWnd;

        bool last = false;
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (last)
            {
                Messenger.Default.Send(new PowerStage.Models.StageMsgMessage() { Msg = "Hello", Visibility = true });
            } else
            {
                Messenger.Default.Send(new PowerStage.Models.StageMsgMessage() { Msg = "This should not be shown", Visibility = false });
            }

            last = !last;
        }

        bool overlay_is_shown = false;
        private void ToggleButton_Click_1(object sender, RoutedEventArgs e)
        {

            if (overlay_is_shown)
            {
                Messenger.Default.Send(new PowerStage.Models.OverlayMessage() { mode = Models.OverlayMessage.OverlayMode.CutToClear });
                overlay_is_shown = false;
                return;
            }

            Messenger.Default.Send(new PowerStage.Models.OverlayMessage() { mode = Models.OverlayMessage.OverlayMode.CutToFreeze });
            overlay_is_shown = true;
        }

        private void ToggleButton_Click_2(object sender, RoutedEventArgs e)
        {

            if (overlay_is_shown)
            {
                Messenger.Default.Send(new PowerStage.Models.OverlayMessage() { mode = Models.OverlayMessage.OverlayMode.CutToClear });
                overlay_is_shown = false;
                return;
            }

            Messenger.Default.Send(new PowerStage.Models.OverlayMessage() { mode = Models.OverlayMessage.OverlayMode.FadeToLogo });
            overlay_is_shown = true;
        }

        private void ToggleButton_Click_3(object sender, RoutedEventArgs e)
        {
            SetForegroundWindow(hWnd);
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            SetForegroundWindow(hWnd);

        }
    }
}
