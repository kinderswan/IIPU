using System;
using System.Collections.Generic;
using System.Text;

namespace USBEject
{
	public class VolumeDeviceClass : DeviceClass
	{
		internal SortedDictionary<string, string> _logicalDrives = new SortedDictionary<string, string>();

		public VolumeDeviceClass() : base(new Guid(Native.GUID_DEVINTERFACE_VOLUME))
		{
			foreach (string drive in Environment.GetLogicalDrives())
			{
				StringBuilder sb = new StringBuilder(1024);
				if (!Native.GetVolumeNameForVolumeMountPoint(drive, sb, sb.Capacity))
				{
					continue;
				}

				this._logicalDrives[sb.ToString()] = drive.Replace("\\", "");
			}
		}

		internal override Device CreateDevice(DeviceClass deviceClass, Native.SP_DEVINFO_DATA deviceInfoData, string path, int index, int disknum = -1)
		{
			return new Volume(deviceClass, deviceInfoData, path, index);
		}
	}
}