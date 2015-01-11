using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AudioInterface;
using AudioCycler;

namespace AudioCyclerConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private CyclerConfig config;
        public MainWindow()
        {
            InitializeComponent();
            this.config = CyclerConfig.Load();
            CyclingDeviceListBox.ItemsSource = config.AllCyclingDevices;
            UncycledDeviceListBox.ItemsSource = config.NonCyclingDevices;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ActivateDevicesButton_OnClick(object sender, RoutedEventArgs e)
        {
            while (UncycledDeviceListBox.SelectedItems.Count > 0)
            {
                AudioDeviceInfo device = (AudioDeviceInfo)UncycledDeviceListBox.SelectedItems[0];
                config.NonCyclingDevices.Remove(device);
                config.AllCyclingDevices.Add(device);
            }
        }

        private void DeactivateDevicesButton_OnClick(object sender, RoutedEventArgs e)
        {
            while (CyclingDeviceListBox.SelectedItems.Count > 0)
            {
                AudioDeviceInfo device = (AudioDeviceInfo)CyclingDeviceListBox.SelectedItems[0];
                config.AllCyclingDevices.Remove(device);
                config.NonCyclingDevices.Add(device);
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            config.Save();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
