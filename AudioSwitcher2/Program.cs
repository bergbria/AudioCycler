using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using AudioInterface;

namespace AudioSwitcher2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var foo = AudioDeviceManager.GetAvailableAudioDevices();
            //AudioDeviceManager.SetDefaultAudioPlaybackDevice(foo[2]);
            //AudioDeviceManager.SetDefaultAudioPlaybackDevice(foo[5]);

            var f = foo.Single();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
