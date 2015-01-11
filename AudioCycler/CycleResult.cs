using AudioInterface;

namespace AudioCycler
{
    public class CycleResult
    {
        public enum CycleResultType
        {
            Success,
            Failure
        }

        public CycleResult()
        {
            ResultType = CycleResultType.Failure;
        }

        public CycleResult(AudioDeviceInfo currentDeviceInfo, CycleResultType resultType, int numCycleableDevices, int relativeDeviceNumber)
        {
            CurrentDeviceInfo = currentDeviceInfo;
            ResultType = resultType;
            NumCycleableDevices = numCycleableDevices;
            RelativeDeviceNumber = relativeDeviceNumber;
        }

        public AudioDeviceInfo CurrentDeviceInfo { get; set; }

        public CycleResultType ResultType{ get; set; }

        public int NumCycleableDevices { get; set; }
        public int RelativeDeviceNumber { get; set; }
    }
}
