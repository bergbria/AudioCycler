#include "stdafx.h"
#include "AudioDeviceInfo.h"

using namespace std;

AudioDeviceInfo::AudioDeviceInfo(wstring deviceName, wstring deviceID)
{
	this->Name = gcnew String(deviceName.c_str());
	this->DeviceId = gcnew String(deviceID.c_str());
}
