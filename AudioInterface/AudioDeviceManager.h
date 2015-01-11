#pragma once

#include "AudioDeviceInfo.h"
#include <mmdeviceapi.h>

namespace AudioInterface {

    public ref class AudioDeviceManager
    {
    public:
        static System::Collections::Generic::List<AudioDeviceInfo^>^ GetAvailableAudioDevices();

        static System::Boolean SetDefaultAudioPlaybackDevice(AudioDeviceInfo^ deviceInfo);

        static AudioDeviceInfo^ GetCurrentAudioPlaybackDevice();

    private:
        static AudioDeviceInfo^ extractDeviceInfo(IMMDevice * deviceInfo);

        //caller is responsible for freeing the returned pointer. The object's Release method should be called previous to this. May return null if the operation fails.
        static IMMDeviceEnumerator* createDeviceEnumerator();
    };
}
