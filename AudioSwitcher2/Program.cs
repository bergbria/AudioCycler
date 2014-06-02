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
            var f = foo.Single();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
