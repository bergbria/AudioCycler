using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioInterface;

namespace AudioSwitcher2
{
    [Serializable]
    public class DeviceCycler
    {
        private List<AudioDeviceInfo> cycleableDevices;

        public DeviceCycler(CyclerConfig config)
        {
            FindCycleableDevices(config);
        }

        private void FindCycleableDevices(CyclerConfig config)
        {
            cycleableDevices = new List<AudioDeviceInfo>();
            List<AudioDeviceInfo> currentDevices = AudioDeviceManager.GetAvailableAudioDevices();
            if (config == null || config.IsEmpty)
            {
                cycleableDevices.AddRange(currentDevices.Where(device => device.Status == DeviceStatus.Active));
            }
            else
            {
                foreach (var storedDevice in config.DevicesToCycle)
                {
                    var currentEquivalent = currentDevices.SingleOrDefault(d => d.DeviceId == storedDevice.DeviceId);
                    if (currentEquivalent != null && currentEquivalent.Status == DeviceStatus.Active)
                    {
                        cycleableDevices.Add(currentEquivalent);
                    }
                }
            }
        }

        public CycleResult Cycle()
        {
            if (cycleableDevices.Any())
            {
                AudioDeviceInfo currentDevice = AudioDeviceManager.GetCurrentAudioPlaybackDevice();
                int currentIndex = cycleableDevices.FindIndex(dev => dev.DeviceId.Equals(currentDevice.DeviceId));
                int newIndex = (currentIndex + 1) % cycleableDevices.Count;
                var resultType = CycleResult.CycleResultType.Success;
                if (currentIndex != newIndex)
                {
                    AudioDeviceInfo newDevice = cycleableDevices[newIndex];
                    bool operationSucceeded = AudioDeviceManager.SetDefaultAudioPlaybackDevice(newDevice);
                    resultType = operationSucceeded ? CycleResult.CycleResultType.Success : CycleResult.CycleResultType.Failure;
                    if (operationSucceeded)
                    {
                        currentDevice = newDevice;
                    }
                }
                return new CycleResult(currentDevice, resultType, cycleableDevices.Count, newIndex);
            }
            return new CycleResult(null, CycleResult.CycleResultType.Failure, -1, -1);
        }
    }
}
