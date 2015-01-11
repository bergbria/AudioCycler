﻿using System;
using System.ComponentModel;
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
