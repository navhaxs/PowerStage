using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using NetOffice;
using System.Drawing;

using Office = NetOffice.OfficeApi;
using NetOffice.OfficeApi.Enums;
using POWERPNT = NetOffice.PowerPointApi;
using NetOffice.PowerPointApi.Enums;
using System.Windows.Forms.Integration;


// Connect.cs
// Implments the COM interface as a PowerPoint Addin
namespace PowerStageAddin
{
    [GuidAttribute("E8786E8A-3AC7-44C7-8BFE-BD1CE21EDCC6")]
    [ProgId("PowerStageAddin.Connect")]
    [ComVisible(true)]
    public class Connect : Extensibility.IDTExtensibility2, Office.IRibbonExtensibility
    {
        public static POWERPNT.Application thisPowerPoint;

        void Extensibility.IDTExtensibility2.OnAddInsUpdate(ref Array custom) {
            //do nothing
        }

        void Extensibility.IDTExtensibility2.OnBeginShutdown(ref Array custom) {
            //do nothing
        }

        void Extensibility.IDTExtensibility2.OnConnection(object Application, Extensibility.ext_ConnectMode ConnectMode, object AddInInst, ref Array custom) {
            //thisPowerPoint = (POWERPNT.Application) Application;
            thisPowerPoint  = new POWERPNT.Application(null, Application);
        }

        void Extensibility.IDTExtensibility2.OnDisconnection(Extensibility.ext_DisconnectMode RemoveMode, ref Array custom) {
            //do nothing
        }

        void Extensibility.IDTExtensibility2.OnStartupComplete(ref Array custom) {
            //do nothing
        }

        // add the app to the Office Ribbon
        public string GetCustomUI(string RibbonID) {
            return Properties.Resources.CustomUI; 
        }

        public Image ReturnImage(Office.IRibbonControl Control) {
            // Since only ONE custom image is used for the addin, simply return that image.
            // Otherwise if multiple images were used, add some code here...
            return Properties.Resources.ribbonButtonLogo;
        }

        public Image ReturnImageFreeze(Office.IRibbonControl Control)
        {
            // Since only ONE custom image is used for the addin, simply return that image.
            // Otherwise if multiple images were used, add some code here...
            return Properties.Resources.ribbonButtonFreeze;
        }

        public bool get_Pressed(Office.IRibbonControl control)

        {

                return true;



        }

        // Using the msi installer rather than relying on regsvr32

        //[ComRegisterFunctionAttribute()]
        //public static void RegisterFunction(Type pType) {

        //}

        //[ComUnregisterFunctionAttribute()]
        //public static void UnregisterFunction(Type pType) {

        //}

        private PowerPointApi pptController;
        // show the main screen
        public void showWiimoteSetup(Office.IRibbonControl control = null, bool wait = false)
        {
            //App.ui = new AppWindow();
            //App.ui.Show();
            pptController = new PowerPointApi(PowerStageAddin.Connect.thisPowerPoint);


            var wpfwindow = new PowerStage.StageDisplayWindow();
            ElementHost.EnableModelessKeyboardInterop(wpfwindow);
            wpfwindow.Show();
        }

        public void showFreeze(Office.IRibbonControl control = null, bool wait = false)
        {
           
                // otherwise create it
                App.ui2s = new ui.overlay();
                App.ui2s.DoFreeze();
                App.ui2s.Show();

            
        }

        public void LaunchRemoveAllWiimotes(Office.IRibbonControl control = null, bool wait = false)
        {
            
        }

        public void LaunchAddWiimotes(Office.IRibbonControl control = null, bool wait = false)
        {

        }
    }
}
