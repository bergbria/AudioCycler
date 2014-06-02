#pragma once
#include <vector>
#include <string>
#include <windows.h>
#include "AudioDeviceInfo.h"
#include <mmdeviceapi.h>

class AudioDeviceManager_native
{
public:
	static std::vector<std::wstring> GetAvailableAudioDevices();

private:
	static AudioDeviceInfo extractDeviceInfo(IMMDevice * deviceInfo);
};

