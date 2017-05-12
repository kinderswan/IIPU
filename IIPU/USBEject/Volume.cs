using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace USBEject
{
	public class Volume : Device
	{
		private int[] _diskNumbers;
		private List<Device> _disks;
		private string _logicalDrive;
		private List<Device> _removableDevices;
		private string _volumeName;

		internal Volume(DeviceClass deviceClass, Native.SP_DEVINFO_DATA deviceInfoData, string path, int index)
			: base(deviceClass, deviceInfoData, path, index)
		{
		}

		public string VolumeName
		{
			get
			{
				if (this._volumeName == null)
				{
					StringBuilder sb = new StringBuilder(1024);
					if (!Native.GetVolumeNameForVolumeMountPoint(this.Path + "\\", sb, sb.Capacity))
					{
					}

					if (sb.Length > 0)
					{
						this._volumeName = sb.ToString();
					}
				}
				return this._volumeName;
			}
		}

		public string LogicalDrive
		{
			get
			{
				if ((this._logicalDrive == null) && (this.VolumeName != null))
				{
					((VolumeDeviceClass) this.DeviceClass)._logicalDrives.TryGetValue(this.VolumeName, out this._logicalDrive);
				}
				return this._logicalDrive;
			}
		}

		public override bool IsUsb
		{
			get { return (this.Disks != null) && this.Disks.Any(disk => disk.IsUsb); }
		}

		public List<Device> Disks
		{
			get
			{
				if (this._disks != null)
				{
					return this._disks;
				}
				this._disks = new List<Device>();

				if (this.DiskNumbers == null)
				{
					return this._disks;
				}
				DiskDeviceClass disks = new DiskDeviceClass();
				foreach (Device disk in this.DiskNumbers.SelectMany(index => disks.Devices.Where(disk => disk.DiskNumber == index)))
				{
					this._disks.Add(disk);
				}
				return this._disks;
			}
		}

		public int[] DiskNumbers
		{
			get
			{
				if (this._diskNumbers != null)
				{
					return this._diskNumbers;
				}
				List<int> numbers = new List<int>();
				if (this.LogicalDrive != null)
				{
					IntPtr hFile = Native.CreateFile(@"\\.\" + this.LogicalDrive, 0, Native.FILE_SHARE_READ | Native.FILE_SHARE_WRITE, IntPtr.Zero, Native.OPEN_EXISTING, 0,
						IntPtr.Zero);
					if (hFile.ToInt32() == Native.INVALID_HANDLE_VALUE)
						throw new Win32Exception(Marshal.GetLastWin32Error());

					int size = 0x400;
					IntPtr buffer = Marshal.AllocHGlobal(size);
					int bytesReturned;
					try
					{
						if (!Native.DeviceIoControl(hFile, Native.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, IntPtr.Zero, 0, buffer, size, out bytesReturned, IntPtr.Zero))
						{
						}
					}
					finally
					{
						Native.CloseHandle(hFile);
					}

					if (bytesReturned > 0)
					{
						int numberOfDiskExtents = (int) Marshal.PtrToStructure(buffer, typeof (int));
						for (int i = 0; i < numberOfDiskExtents; i++)
						{
							IntPtr extentPtr = new IntPtr(buffer.ToInt32() + Marshal.SizeOf(typeof (long)) + i*Marshal.SizeOf(typeof (Native.DISK_EXTENT)));
							Native.DISK_EXTENT extent = (Native.DISK_EXTENT) Marshal.PtrToStructure(extentPtr, typeof (Native.DISK_EXTENT));
							numbers.Add(extent.DiskNumber);
						}
					}
					Marshal.FreeHGlobal(buffer);
				}

				this._diskNumbers = new int[numbers.Count];
				numbers.CopyTo(this._diskNumbers);
				return this._diskNumbers;
			}
		}

		public override List<Device> RemovableDevices
		{
			get
			{
				if (this._removableDevices != null)
				{
					return this._removableDevices;
				}
				this._removableDevices = new List<Device>();
				if (this.Disks == null)
				{
					this._removableDevices = base.RemovableDevices;
				}
				else
				{
					foreach (Device device in this.Disks.SelectMany(disk => disk.RemovableDevices))
					{
						this._removableDevices.Add(device);
					}
				}
				return this._removableDevices;
			}
		}

		public override int CompareTo(object obj)
		{
			Volume device = obj as Volume;
			if (device == null)
				throw new ArgumentException();

			if (this.LogicalDrive == null)
				return 1;

			if (device.LogicalDrive == null)
				return -1;

			return string.Compare(this.LogicalDrive, device.LogicalDrive, StringComparison.Ordinal);
		}
	}
}