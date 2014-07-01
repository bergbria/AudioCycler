using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AudioInterface;

namespace AudioSwitcher2
{
    [Serializable]
    public class CyclerConfig
    {
        public List<AudioDeviceInfo> DevicesToCycle { get; private set; }

        public bool IsEmpty
        {
            get { return !DevicesToCycle.Any(); }
        }

        private CyclerConfig()
        {
            DevicesToCycle = new List<AudioDeviceInfo>();
        }

        private static String SaveFilePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"AudioCycle\cyclerSettings.xml");
            }
        }

        public void Save()
        {
            if (IsEmpty)
            {
                File.Delete(SaveFilePath);
            }
            else
            {
                using (var writeStream = new FileStream(SaveFilePath, FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(CyclerConfig));
                    serializer.Serialize(writeStream, this);
                }
            }
        }

        public static CyclerConfig Load()
        {
            try
            {
                using (var readStream = new FileStream(SaveFilePath, FileMode.Open, FileAccess.Read))
                {
                    var serializer = new XmlSerializer(typeof(CyclerConfig));
                    return serializer.Deserialize(readStream) as CyclerConfig;
                }
            }
            catch (IOException)
            {
                return new CyclerConfig();
            }
        }
    }
}
