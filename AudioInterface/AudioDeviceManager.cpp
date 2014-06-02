#include "stdafx.h"

#include "AudioDeviceManager.h"
#include "AudioDeviceManager_native.h"
#include <wchar.h>
#include <vector>
#include <windows.h>

using namespace std;
using namespace System;
using namespace System::Collections::Generic;

using namespace AudioInterface;

int AudioInterface::AudioDeviceManager::foo() {
	return 5;
}

List<String^>^ AudioDeviceManager::GetAvailableAudioDevices() {
	List<String^>^ deviceNames = gcnew List<String^>();

	vector<wstring> devicesVector = AudioDeviceManager_native::GetAvailableAudioDevices();

	for (int i = 0; i < devicesVector.size(); i++) {
		deviceNames->Add(gcnew String(devicesVector[i].c_str()));
	}
	
	return deviceNames;
}