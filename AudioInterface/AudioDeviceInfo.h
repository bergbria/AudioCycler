#pragma once

using namespace System;

#include <string>

ref class AudioDeviceInfo
{
public:
	AudioDeviceInfo(std::wstring deviceName, std::wstring deviceID);
	AudioDeviceInfo();
	AudioDeviceInfo(const AudioDeviceInfo& other);

	property String ^Name {
		String ^get();
	private:
		void set(String^);
	}

	property String^ DeviceId {
		String^ get();
	private:
		void set(String^);
	}

};

