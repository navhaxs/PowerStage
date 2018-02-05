using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using NetOffice;
using Office = NetOffice.OfficeApi;
using NetOffice.OfficeApi.Enums;
using PowerPoint = NetOffice.PowerPointApi;
using NetOffice.PowerPointApi.Enums;
using System.Diagnostics;
using static PowerStageAddin.Win32;
using PowerStageAddin;
using System.Windows.Forms.Integration;

namespace TestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            KeyboardListener.Register();
            //testAPI();

            //DONT USE:
            //testAddinForm();
            {
                var wpfwindow = new PowerStage.OverlayWindow();
                ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                wpfwindow.Show();
            }
            {
                var wpfwindow = new PowerStage.StageDisplayWindow();
                ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                wpfwindow.Show();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //WiimoteAddin.PowerPointApi wnd2 = new WiimoteAddin.PowerPointApi();
            KeyboardListener.Unregister();
        }

        static public void testAddinForm()
        {
        }

        static public void testAPI()
        {
            // detect presenter view screen
            // draw overlay form on presenter view screen
            // detect powerpoint slideshow screen
            // set up WM_PAINT listener
            // find 3rd free display
            // show form on 3rd display

            //PowerPoint.Application powerApplication = new PowerPoint.Application();
            //// add a new presentation with one new slide
            //PowerPoint.Presentation presentation = powerApplication.Presentations.Add(MsoTriState.msoTrue);
            //presentation.Slides.Add(1, PpSlideLayout.ppLayoutClipArtAndVerticalText);




            Process[] processes = Process.GetProcessesByName("powerpnt");
            Process lol = processes[0];
            IntPtr ptr = lol.MainWindowHandle;
            Rect NotepadRect = new Rect();
            GetWindowRect(ptr, ref NotepadRect);

            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                //if (pList.MainWindowTitle.EndsWith(" - PowerPoint Presenter View"))
                //{
                //    hWnd = pList.MainWindowHandle;
                //}
                if (pList.MainWindowTitle.StartsWith("PowerPoint SlideShow  -  "))
                {
                    hWnd = pList.MainWindowHandle;
                }
            }
            GetWindowRect(hWnd, ref NotepadRect);
            //return hWnd; //Should contain the handle but may be zero if the title doesn't match

        }


    }
}
