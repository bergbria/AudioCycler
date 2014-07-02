using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;


namespace AudioCycler
{

    static class Program
    {
        const string AppId = "Berger.AudioCycler";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DeviceCycler cycler = new DeviceCycler(CyclerConfig.Load());
            var cycleResult = cycler.Cycle();
            DisplayToast(cycleResult);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
        }

        private static void DisplayToast(CycleResult result)
        {
            // Get a toast XML template
            //XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
            XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText03);
            XmlNodeList stringElements = toastXML.GetElementsByTagName("text");

            stringElements[0].AppendChild(toastXML.CreateTextNode("Playing to: " + result.CurrentDeviceInfo.Name));
            stringElements[1].AppendChild(toastXML.CreateTextNode("Device " + (result.RelativeDeviceNumber + 1) + " of " + result.NumCycleableDevices));

            ToastNotification toast = new ToastNotification(toastXML);
            ToastNotificationManager.CreateToastNotifier(AppId).Show(toast);
        }
    }
}
