using System;
using System.IO;
using System.Reflection;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using NotificationsExtensions.ToastContent;

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
            IToastImageAndText03 toastContent = NotificationsExtensions.ToastContent.ToastContentFactory.CreateToastImageAndText03();
            toastContent.TextHeadingWrap.Text = string.Format("Playing to: {0}", result.CurrentDeviceInfo.Name);
            toastContent.TextBody.Text = string.Format("Device {0} of {1}", (result.RelativeDeviceNumber + 1), result.NumCycleableDevices);
            string installDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string imagePath = Path.Combine(installDirectory, "Images", "icon.png");
            toastContent.Image.Src = imagePath;

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(toastContent.ToString());
            ToastNotification toast = new ToastNotification(xml);
            ToastNotificationManager.CreateToastNotifier(AppId).Show(toast);
        }
    }
}
