using GalaSoft.MvvmLight.Messaging;
using PowerStage.Models;
using PowerStageAddin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool mouseIsDown = false;
        private Point firstPoint;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            firstPoint = e.Location;
            mouseIsDown = true;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseIsDown = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                // Get the difference between the two points
                int xDiff = firstPoint.X - e.Location.X;
                int yDiff = firstPoint.Y - e.Location.Y;

                // Set the new point
                int x = this.Location.X - xDiff;
                int y = this.Location.Y - yDiff;
                this.Location = new Point(x, y);
            }
        }
        protected override void WndProc(ref Message m)
        {
            const int wmNcHitTest = 0x84;
            const int htBottomLeft = 16;
            const int htBottomRight = 17;
            if (m.Msg == wmNcHitTest)
            {
                int x = (int)(m.LParam.ToInt64() & 0xFFFF);
                int y = (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16);
                Point pt = PointToClient(new Point(x, y));
                Size clientSize = ClientSize;
                if (pt.X >= clientSize.Width - 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htBottomLeft : htBottomRight);
                    return;
                }
            }
            base.WndProc(ref m);
        }
        
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
        private Bitmap Screenshot()
        {
            Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            Graphics g = Graphics.FromImage(bmpScreenshot);

            g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
            //g.CopyFromScreen(center.X - 36, center.Y - 30, 0, 0, new Size(36 * 2, 30 * 2));

            return bmpScreenshot;
        }

        //private Bitmap ScreenshotPPt()
        //{
        //    var s = PowerStage.PlatformInvokeUSER32.GetWindowTitles(true);
        //    if (s.Count > 0)
        //    {
        //        hnd = s[0];
        //    }
        //    else
        //    {
        //        hnd = IntPtr.Zero;
        //    }
        //    return PowerStage.CaptureScreen.GetWindowImage(hnd);

            //Rect NotepadRect = new Rect();
            //IntPtr hWnd = IntPtr.Zero;
            //foreach (Process pList in Process.GetProcesses())
            //{
            //    //if (pList.MainWindowTitle.EndsWith(" - PowerPoint Presenter View"))
            //    //{
            //    //    hWnd = pList.MainWindowHandle;
            //    //}
            //    if (pList.MainWindowTitle.StartsWith("PowerPoint Slide Show"))
            //    {
            //        hWnd = pList.MainWindowHandle;
            //    }
            //}

            //GetWindowRect(s[0], ref NotepadRect);


        //    Screen targetScreen = Screen.FromHandle(hnd);

        //    if (targetScreen.WorkingArea.Width == 0 || targetScreen.WorkingArea.Height == 0)
        //        return new Bitmap(1,1);

        //    Bitmap bmpScreenshot = new Bitmap(targetScreen.WorkingArea.Width, targetScreen.WorkingArea.Height);
            
        //    Graphics g = Graphics.FromImage(bmpScreenshot);

        //    // assumes top-most...
        //    g.CopyFromScreen(targetScreen.WorkingArea.Left, targetScreen.WorkingArea.Top, 0, 0, targetScreen.WorkingArea.Size);




        //    return bmpScreenshot;
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = PowerStage.CaptureScreen.GetScreenshot(); // CaptureScreen.CaptureScreen.GetDesktopImage();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //pictureBox1.Image = ScreenshotPPt();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private int test = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            Messenger.Default.Send(new SlideProgressUpdateMessage
            {
                CurrentSlideNum = test++,
                TotalSlideNum = test++
            });
        }
    }
}
