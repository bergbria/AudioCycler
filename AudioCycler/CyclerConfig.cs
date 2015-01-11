﻿using System;
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
            get { return AllCyclingDevices.Where(device => device.Status == DeviceStatus.Active).ToList(); }
        }

        public ObservableCollection<AudioDeviceInfo> AllCyclingDevices { get; private set; }

        public ObservableCollection<AudioDeviceInfo> NonCyclingDevices { get; private set; }

        public bool IsEmpty
        {
            get { return !AllCyclingDevices.Any(); }
        }

        private CyclerConfig()
        {
            AllCyclingDevices = new ObservableCollection<AudioDeviceInfo>();
            NonCyclingDevices = new ObservableCollection<AudioDeviceInfo>();
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
            foreach (AudioDeviceInfo savedDevice in savedConfig.AllCyclingDevices)
            {
                AudioDeviceInfo currentEquivalent = currentDevices.SingleOrDefault(d => d.DeviceId == savedDevice.DeviceId);
                if (currentEquivalent == null)
                {
                    savedDevice.Status = DeviceStatus.NotPresent;
                    newConfig.AllCyclingDevices.Add(savedDevice);
                }
                else
                {
                    newConfig.AllCyclingDevices.Add(currentEquivalent);
                }
            }

            IEnumerable<string> allKnownDeviceIds = savedConfig.AllCyclingDevices.Union(savedConfig.NonCyclingDevices).Select(device => device.DeviceId);
            IEnumerable<AudioDeviceInfo> newDevices = currentDevices.Where(device => !allKnownDeviceIds.Contains(device.DeviceId));

            foreach (AudioDeviceInfo device in newDevices)
            {
                newConfig.AllCyclingDevices.Add(device);
            }
            newConfig.NonCyclingDevices = savedConfig.NonCyclingDevices;
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
