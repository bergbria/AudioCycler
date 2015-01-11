using System;
using System.Collections.Generic;
using AudioInterface;

namespace AudioCycler
{
    [Serializable]
    public class DeviceCycler
    {
        private CyclerConfig Config;

        public DeviceCycler(CyclerConfig config)
        {
            FindCycleableDevices(config);
        }

        private void FindCycleableDevices(CyclerConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException();
            }
            this.Config = config;
        }

        public CycleResult Cycle()
        {
            if (!Config.IsEmpty)
            {
                List<AudioDeviceInfo> cyclingDevices = Config.ActiveCyclingDevices;
                AudioDeviceInfo currentDevice = AudioDeviceManager.GetCurrentAudioPlaybackDevice();
                int currentIndex = cyclingDevices.FindIndex(dev => dev.DeviceId.Equals(currentDevice.DeviceId));
                int newIndex = (currentIndex + 1) % cyclingDevices.Count;
                CycleResult.CycleResultType resultType = CycleResult.CycleResultType.Success;
                if (currentIndex != newIndex)
                {
                    AudioDeviceInfo newDevice = cyclingDevices[newIndex];
                    bool operationSucceeded = AudioDeviceManager.SetDefaultAudioPlaybackDevice(newDevice);
                    resultType = operationSucceeded ? CycleResult.CycleResultType.Success : CycleResult.CycleResultType.Failure;
                    if (operationSucceeded)
                    {
                        currentDevice = newDevice;
                    }
                }
                return new CycleResult(currentDevice, resultType, cyclingDevices.Count, newIndex);
            }
            return new CycleResult(null, CycleResult.CycleResultType.Failure, -1, -1);
        }
    }
}
