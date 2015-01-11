#include "stdafx.h"

#include "AudioDeviceManager.h"
#include <wchar.h>
#include <windows.h>
#include <PropIdl.h>
#include <string>
#include <functiondiscoverykeys_devpkey.h>
#include <msclr\marshal.h>
#include "PolicyConfig.h"

using namespace std;
using namespace System;
using namespace System::Collections::Generic;
using namespace msclr::interop;

using namespace AudioInterface;

IMMDeviceEnumerator* AudioDeviceManager::createDeviceEnumerator() {
    HRESULT result;
    IMMDeviceEnumerator *pEnum = NULL;
    // Create a multimedia device enumerator.
    result = CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL,
        CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**) &pEnum);
    if (SUCCEEDED(result)) {
        return pEnum;
    }
    return NULL;
}

List<AudioDeviceInfo^>^ AudioDeviceManager::GetAvailableAudioDevices() {
    List<AudioDeviceInfo^>^ result = gcnew List<AudioDeviceInfo^>();
    HRESULT hr = CoInitialize(NULL);
    if (SUCCEEDED(hr))
    {
        IMMDeviceEnumerator * pEnum = createDeviceEnumerator();
        if (pEnum != NULL)
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
                            AudioDeviceInfo^ deviceInfo = extractDeviceInfo(pDevice);
                            if (deviceInfo != nullptr) {
                                result->Add(deviceInfo);
                            }
                            pDevice->Release();
                        }
                    }
                }
                pDevices->Release();
            }

            //WTF?  If Release() gets called, then redirecting this .exe's output stops working. No idea why.. 
            pEnum->Release();
        }
    }
    return result;
}

AudioDeviceInfo^ AudioDeviceManager::GetCurrentAudioPlaybackDevice() {
    HRESULT result;
    IMMDevice * device;

    IMMDeviceEnumerator * deviceEnumerator = createDeviceEnumerator();

    result = deviceEnumerator->GetDefaultAudioEndpoint(eRender, eMultimedia, &device);
    if (!SUCCEEDED(result)) {
        return nullptr;
    }

    AudioDeviceInfo^ managedDevice = extractDeviceInfo(device);

    deviceEnumerator->Release();
    return managedDevice;
}

System::Boolean AudioDeviceManager::SetDefaultAudioPlaybackDevice(AudioDeviceInfo^ deviceInfo) {
    marshal_context context;
    LPCTSTR deviceId = context.marshal_as<const TCHAR*>(deviceInfo->DeviceId);

    IPolicyConfigVista *pPolicyConfig;
    ERole reserved = eConsole;

    HRESULT hr = CoCreateInstance(__uuidof(CPolicyConfigVistaClient),
        NULL, CLSCTX_ALL, __uuidof(IPolicyConfigVista), (LPVOID *) &pPolicyConfig);
    bool success = SUCCEEDED(hr);
    if (success)
    {
        hr = pPolicyConfig->SetDefaultEndpoint(deviceId, reserved);
        pPolicyConfig->Release();
    }
    return Boolean(success);
}

AudioDeviceInfo^ AudioDeviceManager::extractDeviceInfo(IMMDevice * deviceInfo) {
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
                DWORD deviceState;
                deviceInfo->GetState(&deviceState);
                AudioDeviceInfo^ result = gcnew AudioDeviceInfo(wstring(friendlyName.pwszVal), wstring(wstrID), deviceState);

                PropVariantClear(&friendlyName);
                pStore->Release();
                return result;
            }
            pStore->Release();
        }
    }
    return nullptr;
}