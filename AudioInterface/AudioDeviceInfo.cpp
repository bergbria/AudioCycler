#include "stdafx.h"
#include "AudioDeviceInfo.h"
#include <mmdeviceapi.h>

using namespace std;
using namespace System;
using namespace AudioInterface;

AudioDeviceInfo::AudioDeviceInfo() {

}

AudioDeviceInfo::AudioDeviceInfo(const wstring & deviceName, const wstring & deviceID, const DWORD& deviceState)
{
    this->Name = gcnew String(deviceName.c_str());
    this->DeviceId = gcnew String(deviceID.c_str());
    this->Status = DetermineDeviceStatus(deviceState);
}

String^ AudioDeviceInfo::ToString() {
    return this->Name;
}

DeviceStatus AudioDeviceInfo::DetermineDeviceStatus(const DWORD& deviceStatus) {

    DeviceStatus status;

    if (deviceStatus & DEVICE_STATE_ACTIVE) {
        status = status | DeviceStatus::Active;
    }
    if (deviceStatus & DEVICE_STATE_DISABLED) {
        status = status | DeviceStatus::Disabled;
    }
    if (deviceStatus & DEVICE_STATE_UNPLUGGED) {
        status = status | DeviceStatus::Unplugged;
    }
    if (deviceStatus & DEVICE_STATE_NOTPRESENT) {
        status = status | DeviceStatus::NotPresent;
    }
    return status;
}