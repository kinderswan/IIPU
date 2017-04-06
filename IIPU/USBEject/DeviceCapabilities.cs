using System;

namespace USBEject
{
	[Flags]
	public enum DeviceCapabilities
	{
		Unknown = 0x00000000,
		LockSupported = 0x00000001,
		EjectSupported = 0x00000002,
		Removable = 0x00000004,
		DockDevice = 0x00000008,
		UniqueId = 0x00000010,
		SilentInstall = 0x00000020,
		RawDeviceOk = 0x00000040,
		SurpriseRemovalOk = 0x00000080,
		HardwareDisabled = 0x00000100,
		NonDynamic = 0x00000200
	}
}