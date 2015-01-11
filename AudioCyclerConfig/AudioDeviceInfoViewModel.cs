using System.ComponentModel;
using System.Runtime.CompilerServices;
using AudioInterface;

namespace AudioCyclerConfig
{
    public class AudioDeviceInfoViewModel : INotifyPropertyChanged
    {
        private AudioDeviceInfo _deviceInfo;
        private bool _isVisible;
        private bool _isDefaultDevice;

        public AudioDeviceInfo DeviceInfo
        {
            get { return _deviceInfo; }
            set { _deviceInfo = value; OnPropertyChanged(); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; OnPropertyChanged(); }
        }

        public bool IsDefaultDevice
        {
            get { return _isDefaultDevice; }
            set { _isDefaultDevice = value; OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
