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
                var wpfwindow = new PowerStage.ToolbarWindow();
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

   
    }
}
