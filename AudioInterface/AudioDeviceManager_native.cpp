#include "stdafx.h"
#include "AudioDeviceManager_native.h"

#include <PropIdl.h>
#include <functiondiscoverykeys_devpkey.h>

using namespace std;

vector<wstring> AudioDeviceManager_native::GetAvailableAudioDevices() {
	vector<wstring> result;
	HRESULT hr = CoInitialize(NULL);
	if (SUCCEEDED(hr))
	{
		IMMDeviceEnumerator *pEnum = NULL;
		// Create a multimedia device enumerator.
		hr = CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL,
			CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**) &pEnum);
		if (SUCCEEDED(hr))
		{
			IMMDeviceCollection *pDevices;
			// Enumerate the output devices.
			hr = pEnum->EnumAudioEndpoints(eRender, DEVICE_STATE_ACTIVE | DEVICE_STATE_UNPLUGGED | DEVICE_STATE_DISABLED, &pDevices);
			if (SUCCEEDED(hr))
			{
				UINT count;
				pDevices->GetCount(&count);
				if (SUCCEEDED(hr))
				{
					for (UINT i = 0; i < count; i++)
					{
						IMMDevice *pDevice;
						hr = pDevices->Item(i, &pDevice);
						if (SUCCEEDED(hr))
						{
							wstring name = extractFriendlyName(pDevice);
							if (!name.empty()) {
								result.push_back(name);
							}
							pDevice->Release();
						}
					}
				}
				pDevices->Release();
			}

			//WTF?  If Release() gets called, then redirecting this .exe's output stops working. No idea why.. 
			// pEnum->Release();
		}
	}
	return result;
}

AudioDeviceInfo AudioDeviceManager_native::extractDeviceInfo(IMMDevice * deviceInfo) {
	LPWSTR wstrID = NULL;
	HRESULT hr = deviceInfo->GetId(&wstrID);
	if (SUCCEEDED(hr))
	{
		IPropertyStore *pStore;
		hr = deviceInfo->OpenPropertyStore(STGM_READ, &pStore);
		if (SUCCEEDED(hr))
		{
			PROPVARIANT friendlyName;
			PropVariantInit(&friendlyName);
			hr = pStore->GetValue(PKEY_Device_FriendlyName, &friendlyName);
			if (SUCCEEDED(hr))
			{
				AudioDeviceInfo result(wstring(friendlyName.pwszVal), wstring(wstrID));

				PropVariantClear(&friendlyName);
				pStore->Release();
				return result;
			}
			pStore->Release();
		}
	}
	return AudioDeviceInfo();
}