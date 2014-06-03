#pragma once

#include <string>
#include <Windows.h>

namespace AudioInterface {
	[System::Flags]
	public enum class DeviceStatus {
		Active = 1, 
		Disabled = 2,
		Unplugged = 4,
		NotPresent = 8
	};

	public ref class AudioDeviceInfo
	{
	public:
		AudioDeviceInfo(const std::wstring & deviceName, const std::wstring & deviceID, const DWORD& deviceState);

		property System::String ^Name;

		property System::String^ DeviceId;

		property DeviceStatus Status;

		virtual System::String^ ToString() override;

	private:
		static DeviceStatus DetermineDeviceStatus(const DWORD& deviceStatus);
	};
}