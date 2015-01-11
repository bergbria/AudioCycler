using System;
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
            CycleResult cycleResult = cycler.Cycle();
            DisplayToast(cycleResult);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
        }

        private static void DisplayToast(CycleResult result)
        {
            // Get a toast XML template
            //XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
            XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText03);
            XmlNodeList stringElements = toastXML.GetElementsByTagName("text");

            stringElements[0].AppendChild(toastXML.CreateTextNode("Playing to: " + result.CurrentDeviceInfo.Name));
            stringElements[1].AppendChild(toastXML.CreateTextNode("Device " + (result.RelativeDeviceNumber + 1) + " of " + result.NumCycleableDevices));

            XmlNodeList images = toastXML.GetElementsByTagName("image");
            //images[0].Attributes.SetNamedItem()
            //images[0].setAttribute("src", "images/toastImageAndText.png");

            String imagePath = "file:///" + @"E:\Users\Brian\Code\Personal\AudioCycler\img\icon.png";
            XmlNodeList imageElements = toastXML.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            ToastNotification toast = new ToastNotification(toastXML);
            ToastNotificationManager.CreateToastNotifier(AppId).Show(toast);
        }
    }
}
