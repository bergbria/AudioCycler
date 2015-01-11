using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using AudioCycler;
using AudioInterface;

namespace AudioCyclerConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private TrulyObservableCollection<AudioDeviceInfoViewModel> _displayedCyclingDevices;
        private TrulyObservableCollection<AudioDeviceInfoViewModel> _displayedNonCyclingDevices;

        public MainWindow()
        {
            InitializeComponent();
            CyclerConfig config = CyclerConfig.Load();
            InitializeDisplayedDevices(config);

            this.CyclingDeviceListBox.ItemsSource = _displayedCyclingDevices;
            this.NonCyclingDeviceListBox.ItemsSource = _displayedNonCyclingDevices;
        }

        private void InitializeDisplayedDevices(CyclerConfig config)
        {
            _displayedCyclingDevices = new TrulyObservableCollection<AudioDeviceInfoViewModel>();
            foreach (AudioDeviceInfo cyclingDevice in config.CyclingDevices)
            {
                _displayedCyclingDevices.Add(CreateDeviceViewModel(cyclingDevice));
            }

            _displayedNonCyclingDevices = new TrulyObservableCollection<AudioDeviceInfoViewModel>();
            foreach (AudioDeviceInfo nonCyclingDevice in config.NonCyclingDevices)
            {
                _displayedNonCyclingDevices.Add(CreateDeviceViewModel(nonCyclingDevice));
            }

            MarkDefaultDevice();
        }

        private static AudioDeviceInfoViewModel CreateDeviceViewModel(AudioDeviceInfo cyclingDevice)
        {
            return new AudioDeviceInfoViewModel
            {
                DeviceInfo = cyclingDevice,
                IsVisible = true,
                IsDefaultDevice = false,
            };
        }

        private void MarkDefaultDevice()
        {
            AudioDeviceInfo defaultDevice = AudioDeviceManager.GetCurrentAudioPlaybackDevice();
            if (defaultDevice != null)
            {
                string defaultDeviceId = defaultDevice.DeviceId;
                AudioDeviceInfoViewModel defaultDeviceViewModel = _displayedCyclingDevices
                    .Union(_displayedNonCyclingDevices)
                    .SingleOrDefault(device => device.DeviceInfo.DeviceId == defaultDeviceId);

                if (defaultDeviceViewModel != null)
                {
                    defaultDeviceViewModel.IsDefaultDevice = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ActivateDevicesButton_OnClick(object sender, RoutedEventArgs e)
        {
            while (NonCyclingDeviceListBox.SelectedItems.Count > 0)
            {
                AudioDeviceInfoViewModel device = (AudioDeviceInfoViewModel)NonCyclingDeviceListBox.SelectedItems[0];
                _displayedNonCyclingDevices.Remove(device);
                _displayedCyclingDevices.Add(device);
            }
        }

        private void DeactivateDevicesButton_OnClick(object sender, RoutedEventArgs e)
        {
            while (CyclingDeviceListBox.SelectedItems.Count > 0)
            {
                AudioDeviceInfoViewModel device = (AudioDeviceInfoViewModel)CyclingDeviceListBox.SelectedItems[0];
                _displayedCyclingDevices.Remove(device);
                _displayedNonCyclingDevices.Add(device);
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            IEnumerable<AudioDeviceInfo> cyclingDevices = _displayedCyclingDevices.Select(d => d.DeviceInfo);
            IEnumerable<AudioDeviceInfo> nonCyclingDevices = _displayedNonCyclingDevices.Select(d => d.DeviceInfo);

            CyclerConfig config = new CyclerConfig(cyclingDevices, nonCyclingDevices);
            config.Save();
            this.Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowOnlyActiveDevicesCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            IEnumerable<AudioDeviceInfoViewModel> inactiveDevices =
              _displayedCyclingDevices.Union(_displayedNonCyclingDevices).Where(device => device.DeviceInfo.Status != DeviceStatus.Active);

            foreach (AudioDeviceInfoViewModel device in inactiveDevices)
            {
                device.IsVisible = false;
            }

            CyclingDeviceListBox.SelectedItems.Clear();
        }

        private void ShowOnlyActiveDevicesCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            IEnumerable<AudioDeviceInfoViewModel> inactiveDevices =
                _displayedCyclingDevices.Union(_displayedNonCyclingDevices).Where(device => !device.IsVisible);

            foreach (AudioDeviceInfoViewModel device in inactiveDevices)
            {
                device.IsVisible = true;
            }
        }

        private void MoveUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            IEnumerable<int> selectedIndices = GetSortedSelectedCyclingDeviceIdices(false);
            foreach (int deviceIndex in selectedIndices)
            {
                if (deviceIndex > 0)
                {
                    int precedingIndex = deviceIndex - 1;
                    AudioDeviceInfoViewModel device = _displayedCyclingDevices[deviceIndex];
                    _displayedCyclingDevices[deviceIndex] = _displayedCyclingDevices[precedingIndex];
                    _displayedCyclingDevices[precedingIndex] = device;
                    CyclingDeviceListBox.SelectedItems.Add(device);
                }
            }
        }

        private void MoveDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            IEnumerable<int> selectedIndices = GetSortedSelectedCyclingDeviceIdices(true);
            foreach (int deviceIndex in selectedIndices)
            {
                if (deviceIndex < _displayedCyclingDevices.Count - 1)
                {
                    int followingIndex = deviceIndex + 1;
                    AudioDeviceInfoViewModel device = _displayedCyclingDevices[deviceIndex];
                    _displayedCyclingDevices[deviceIndex] = _displayedCyclingDevices[followingIndex];
                    _displayedCyclingDevices[followingIndex] = device;
                    CyclingDeviceListBox.SelectedItems.Add(device);
                }
            }
        }

        private IEnumerable<int> GetSortedSelectedCyclingDeviceIdices(bool sortDescending)
        {
            IEnumerable<int> selectedIndices = CyclingDeviceListBox.SelectedItems.Cast<AudioDeviceInfoViewModel>().Select(
                device => _displayedCyclingDevices.IndexOf(device));

            if (sortDescending)
            {
                return selectedIndices.OrderByDescending(index => index);
            }

            return selectedIndices.OrderBy(index => index);
        }
    }
}
