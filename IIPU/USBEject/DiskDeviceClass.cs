using System;

namespace USBEject
{
	public class DiskDeviceClass : DeviceClass
	{
		public DiskDeviceClass() : base(new Guid(Native.GUID_DEVINTERFACE_DISK))
		{
		}
	}
}