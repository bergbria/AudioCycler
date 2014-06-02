#pragma once

namespace AudioInterface {

	public ref class AudioDeviceManager
	{
	public:
		int foo();
		static System::Collections::Generic::List<System::String^>^ GetAvailableAudioDevices();
	};
}
