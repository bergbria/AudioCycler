using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AudioInterface;

namespace AudioCycler
{
    [Serializable]
    public class CyclerConfig
    {
        public List<AudioDeviceInfo> ActiveCyclingDevices
        {
            get { return CyclingDevices.Where(device => device.Status == DeviceStatus.Active).ToList(); }
        }

        public List<AudioDeviceInfo> CyclingDevices { get; private set; }

        public List<AudioDeviceInfo> NonCyclingDevices { get; private set; }

        public bool IsEmpty
        {
            get { return !CyclingDevices.Any(); }
        }

        private CyclerConfig()
        {
            CyclingDevices = new List<AudioDeviceInfo>();
            NonCyclingDevices = new List<AudioDeviceInfo>();
        }

        public CyclerConfig(IEnumerable<AudioDeviceInfo> cyclingDevices, IEnumerable<AudioDeviceInfo> nonCyclingDevices)
        {
            this.CyclingDevices = new List<AudioDeviceInfo>(cyclingDevices);
            this.NonCyclingDevices = new List<AudioDeviceInfo>(nonCyclingDevices);
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
            Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath));
            if (IsEmpty)
            {
                File.Delete(SaveFilePath);
            }
            else
            {
                using (FileStream writeStream = new FileStream(SaveFilePath, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CyclerConfig));
                    serializer.Serialize(writeStream, this);
                }
            }
        }

        public static CyclerConfig Load()
        {
            CyclerConfig newConfig = new CyclerConfig();
            CyclerConfig savedConfig = RetrieveSavedConfig();
            List<AudioDeviceInfo> currentDevices = AudioDeviceManager.GetAvailableAudioDevices();
            foreach (AudioDeviceInfo savedDevice in savedConfig.CyclingDevices)
            {
                AudioDeviceInfo currentEquivalent = currentDevices.SingleOrDefault(d => d.DeviceId == savedDevice.DeviceId);
                if (currentEquivalent == null)
                {
                    savedDevice.Status = DeviceStatus.NotPresent;
                    newConfig.CyclingDevices.Add(savedDevice);
                }
                else
                {
                    newConfig.CyclingDevices.Add(currentEquivalent);
                }
            }

            IEnumerable<string> allKnownDeviceIds = savedConfig.CyclingDevices.Union(savedConfig.NonCyclingDevices).Select(device => device.DeviceId);
            IEnumerable<AudioDeviceInfo> newDevices = currentDevices.Where(device => !allKnownDeviceIds.Contains(device.DeviceId));

            newConfig.CyclingDevices.AddRange(newDevices);

            newConfig.NonCyclingDevices = new List<AudioDeviceInfo>(
                currentDevices.Where(currentDevice => !newConfig.CyclingDevices
                    .Any(activeCyclingDevice => activeCyclingDevice.DeviceId == currentDevice.DeviceId)));

            return newConfig;
        }

        private static CyclerConfig RetrieveSavedConfig()
        {
            CyclerConfig config = new CyclerConfig();
            try
            {
                using (FileStream readStream = new FileStream(SaveFilePath, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CyclerConfig));
                    config = serializer.Deserialize(readStream) as CyclerConfig;
                }
            }
            catch (IOException)
            {
            }

            return config;
        }
    }
}
