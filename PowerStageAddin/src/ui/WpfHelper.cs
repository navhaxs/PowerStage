using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace PowerStageAddin.src.ui
{
    class WpfHelper
    {
        public static void Startup()
        {
            //var appDomainSetup = new AppDomainSetup()
            //{
            //    ApplicationBase = Path.GetDirectoryName(typeof(WpfHelper).GetType().Assembly.Location)
            //};

            //AppDomain domain = AppDomain.CreateDomain(DateTime.Now.ToString(), null, appDomainSetup);

            //CrossAppDomainDelegate action = () =>
            //{
            //    Thread thread = new Thread(() =>
            //    {
            //        var app = new App();
            //        App.ResourceAssembly = Application.GetAssembly(typeof(App));

            //        app.OverlayWindow = new PowerStageAddin.OverlayWindow();
            //        app.OverlayWindow.Show();
            //        app.Run();
            //    });

            //    thread.SetApartmentState(ApartmentState.STA);
            //    thread.Start();

            //};

            //domain.DoCallBack(action);
        }
    }
}
