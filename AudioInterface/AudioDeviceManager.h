#pragma once

#include "AudioDeviceInfo.h"
#include <mmdeviceapi.h>

namespace AudioInterface {

	public ref class AudioDeviceManager
	{
	public:
		static System::Collections::Generic::List<AudioDeviceInfo^>^ GetAvailableAudioDevices();

		static System::Boolean SetDefaultAudioPlaybackDevice(AudioDeviceInfo^ deviceInfo);

	private:
		static AudioDeviceInfo^ extractDeviceInfo(IMMDevice * deviceInfo);
	};
}
