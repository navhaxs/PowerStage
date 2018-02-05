using GalaSoft.MvvmLight.Messaging;
using PowerStage.Models;
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
using static PowerStageAddin.Win32;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var s = PowerStageAddin.MyEnumWindows.GetWindowTitles(true);
            if (s.Count > 0)
            {
                hnd = s[0];
            } else
            {
                hnd = IntPtr.Zero;
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

        IntPtr hnd;


        private Bitmap ScreenshotPPt()
        {

            return PrintWindow(hnd);

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


            Screen targetScreen = Screen.FromHandle(hnd);

            if (targetScreen.WorkingArea.Width == 0 || targetScreen.WorkingArea.Height == 0)
                return new Bitmap(1,1);

            Bitmap bmpScreenshot = new Bitmap(targetScreen.WorkingArea.Width, targetScreen.WorkingArea.Height);
            
            Graphics g = Graphics.FromImage(bmpScreenshot);

            // assumes top-most...
            g.CopyFromScreen(targetScreen.WorkingArea.Left, targetScreen.WorkingArea.Top, 0, 0, targetScreen.WorkingArea.Size);




            return bmpScreenshot;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = ScreenshotPPt();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = ScreenshotPPt();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PowerStageAddin.Overlay s = new PowerStageAddin.Overlay();
            s.Show();
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
