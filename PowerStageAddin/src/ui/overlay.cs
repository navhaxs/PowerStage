using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace PowerStageAddin.ui
{
    public partial class overlay: Form
    {

        public void DoFreeze() {
            //string matchPattern = " - PowerPoint Presenter View";
            string matchPattern = "PowerPoint Slide Show  -"; // for some reason, MS decided to use two spaces for this one..
            IntPtr matchedHandle;
            string matchedTitle;
            FindWindowTitleMatch(matchPattern, out matchedHandle, out matchedTitle);
            Bitmap screenshot = PowerStage.CaptureScreen.GetScreenshot();
            pictureBox1.Image = screenshot;
        }

        // The target window's pattern to match
        // and the found handle and title.
        private static string MatchPattern;
        private static IntPtr MatchedHandle;
        private static string MatchedTitle; 

        // Return a list of the desktop windows' handles and titles.
        public static void FindWindowTitleMatch(
           string pattern, out IntPtr handle, out string title)
        {
            MatchPattern = pattern;
            MatchedHandle = IntPtr.Zero;
            MatchedTitle = "";

            Win32Api.EnumDesktopWindows(IntPtr.Zero, FilterCallback, IntPtr.Zero);

            handle = MatchedHandle;
            title = MatchedTitle;
        }

        // Select the first window that matches the target pattern.
        private static bool FilterCallback(IntPtr hWnd, int lParam)
        {
            // Get the window's title.
            StringBuilder sb_title = new StringBuilder(1024);
            int length = Win32Api.GetWindowText(hWnd, sb_title, sb_title.Capacity);
            string title = sb_title.ToString();

            // If the window is visible and has a title, see if it matches.
            if (Win32Api.IsWindowVisible(hWnd) &&
                string.IsNullOrEmpty(title) == false)
            {
                System.Diagnostics.Debug.Print(title);
                if (title.StartsWith(MatchPattern))
                {
                    MatchedHandle = hWnd;
                    MatchedTitle = title;
                    System.Diagnostics.Debug.Print("OK");

                    // Return false to indicate that we
                    // don't need to continue enumerating windows.
                    return false;
                }
            }

            // Return true to indicate that we
            // should continue enumerating windows.
            return true;
        }

        public Bitmap GetScreenshot(IntPtr hwnd)
        {
            Win32Api.RECT rc;
            Win32Api.GetWindowRect(new HandleRef(null, hwnd), out rc);

            if (hwnd == IntPtr.Zero) throw new Exception("hwnd is null"); // XXX return black rectangle. gracefully handle error

            Bitmap bmp = new Bitmap(rc.Right - rc.Left, rc.Bottom - rc.Top, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap;
            try
            {
                hdcBitmap = gfxBmp.GetHdc();
            }
            catch
            {
                return null;
            }
            bool succeeded = Win32Api.PrintWindow(hwnd, hdcBitmap, 0);
            gfxBmp.ReleaseHdc(hdcBitmap);
            if (!succeeded)
            {
                gfxBmp.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(Point.Empty, bmp.Size));
            }
            IntPtr hRgn = Win32Api.CreateRectRgn(0, 0, 0, 0);
            Win32Api.GetWindowRgn(hwnd, hRgn);
            Region region = Region.FromHrgn(hRgn);//err here once
            if (!region.IsEmpty(gfxBmp))
            {
                gfxBmp.ExcludeClip(region);
                gfxBmp.Clear(Color.Transparent);
            }
            gfxBmp.Dispose();
            return bmp;
        }

        public void freeze()
        {
            

        }

        public overlay()
        {
            InitializeComponent();
            this.Opacity = 0;

            // XXX assumes there is (at least) a dual display
            this.Location = Screen.AllScreens[1].WorkingArea.Location; // XXX find me from PPTFrameClass

        }

        private async void FadeIn(Form o, int interval = 80)
        {
            //Object is not fully invisible. Fade it in
            while (o.Opacity < 1.0)
            {
                await Task.Delay(interval);
                o.Opacity += 0.05;
            }
            o.Opacity = 1; //make fully visible       
        }

        private void FadeOut(Form o, int interval = 80)
        {
            //Object is fully visible. Fade it out

            o.Opacity = 0; //make fully invisible       
            Close();   //and we try to close the form
        }

        private void overlay_Shown(object sender, EventArgs e)
        {
            FadeIn(this, 1);

        }

        Timer t1 = new Timer();
        private void overlay_FormClosing(object sender, FormClosingEventArgs e)
        {                e.Cancel = true;    //cancel the event so the form won't be closed

                t1.Interval = 1;
                t1.Tick += new EventHandler(fadeOut);  //this calls the fade out function
                t1.Start();

                if (Opacity == 0)  //if the form is completly transparent
                    e.Cancel = false;   //resume the event - the program can be closed

            }

            void fadeOut(object sender, EventArgs e)
            {
                if (Opacity <= 0)     //check if opacity is 0
                {
                    t1.Stop();    //if it is, we stop the timer
                    Close();   //and we try to close the form
                }
                else
                    Opacity -= 0.05;
            }
        }
}
