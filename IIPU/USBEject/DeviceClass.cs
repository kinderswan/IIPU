using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace USBEject
{
	public abstract class DeviceClass : IDisposable
	{
		private Guid _classGuid;
		private IntPtr _deviceInfoSet;
		private List<Device> _devices;

		protected DeviceClass(Guid classGuid) : this(classGuid, IntPtr.Zero)
		{
		}

		protected DeviceClass(Guid classGuid, IntPtr hwndParent)
		{
			this._classGuid = classGuid;

			this._deviceInfoSet = Native.SetupDiGetClassDevs(ref this._classGuid, 0, hwndParent, Native.DIGCF_DEVICEINTERFACE | Native.DIGCF_PRESENT);
			if (this._deviceInfoSet.ToInt32() == Native.INVALID_HANDLE_VALUE)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		public Guid ClassGuid
		{
			get { return this._classGuid; }
		}

		public List<Device> Devices
		{
			get
			{
				if (this._devices != null)
				{
					return this._devices;
				}

				this._devices = new List<Device>();
				int index = 0;
				while (true)
				{
					Native.SP_DEVICE_INTERFACE_DATA interfaceData = new Native.SP_DEVICE_INTERFACE_DATA();

					if (!Native.SetupDiEnumDeviceInterfaces(this._deviceInfoSet, null, ref this._classGuid, index, interfaceData))
					{
						int error = Marshal.GetLastWin32Error();
						if (error != Native.ERROR_NO_MORE_ITEMS)
							throw new Win32Exception(error);
						break;
					}

					Native.SP_DEVINFO_DATA devData = new Native.SP_DEVINFO_DATA();
					int size = 0;
					if (!Native.SetupDiGetDeviceInterfaceDetail(this._deviceInfoSet, interfaceData, IntPtr.Zero, 0, ref size, devData))
					{
						int error = Marshal.GetLastWin32Error();
						if (error != Native.ERROR_INSUFFICIENT_BUFFER)
							throw new Win32Exception(error);
					}

					IntPtr buffer = Marshal.AllocHGlobal(size);
					Native.SP_DEVICE_INTERFACE_DETAIL_DATA detailData = new Native.SP_DEVICE_INTERFACE_DETAIL_DATA
					{
						cbSize = Marshal.SizeOf(typeof (Native.SP_DEVICE_INTERFACE_DETAIL_DATA))
					};
					Marshal.StructureToPtr(detailData, buffer, false);

					if (!Native.SetupDiGetDeviceInterfaceDetail(this._deviceInfoSet, interfaceData, buffer, size, ref size, devData))
					{
						Marshal.FreeHGlobal(buffer);
						throw new Win32Exception(Marshal.GetLastWin32Error());
					}

					IntPtr pDevicePath = (IntPtr) ((int) buffer + Marshal.SizeOf(typeof (int)));
					string devicePath = Marshal.PtrToStringAuto(pDevicePath);
					Marshal.FreeHGlobal(buffer);

					if (this._classGuid.Equals(new Guid(Native.GUID_DEVINTERFACE_DISK)))
					{
						IntPtr hFile = Native.CreateFile(devicePath, 0, Native.FILE_SHARE_READ | Native.FILE_SHARE_WRITE, IntPtr.Zero, Native.OPEN_EXISTING, 0, IntPtr.Zero);
						if (hFile.ToInt32() == Native.INVALID_HANDLE_VALUE)
							throw new Win32Exception(Marshal.GetLastWin32Error());

						int bytesReturned = 0;
						int numBufSize = 0x400;
						IntPtr numBuffer = Marshal.AllocHGlobal(numBufSize);
						Native.STORAGE_DEVICE_NUMBER disknum;

						try
						{
							Native.DeviceIoControl(hFile, Native.IOCTL_STORAGE_GET_DEVICE_NUMBER, IntPtr.Zero, 0, numBuffer, numBufSize, out bytesReturned, IntPtr.Zero);
						}
						finally
						{
							Native.CloseHandle(hFile);
						}

						if (bytesReturned > 0)
							disknum = (Native.STORAGE_DEVICE_NUMBER) Marshal.PtrToStructure(numBuffer, typeof (Native.STORAGE_DEVICE_NUMBER));
						else
							disknum = new Native.STORAGE_DEVICE_NUMBER {DeviceNumber = -1, DeviceType = -1, PartitionNumber = -1};

						Device device = this.CreateDevice(this, devData, devicePath, index, disknum.DeviceNumber);
						this._devices.Add(device);

						Marshal.FreeHGlobal(hFile);
					}
					else
					{
						Device device = this.CreateDevice(this, devData, devicePath, index);
						this._devices.Add(device);
					}

					index++;
				}
				this._devices.Sort();
				return this._devices;
			}
		}

		public void Dispose()
		{
			if (this._deviceInfoSet == IntPtr.Zero)
			{
				return;
			}

			Native.SetupDiDestroyDeviceInfoList(this._deviceInfoSet);
			this._deviceInfoSet = IntPtr.Zero;
		}

		internal virtual Device CreateDevice(DeviceClass deviceClass, Native.SP_DEVINFO_DATA deviceInfoData, string path, int index, int disknum = -1)
		{
			return new Device(deviceClass, deviceInfoData, path, index, disknum);
		}

		internal Native.SP_DEVINFO_DATA GetInfo(int dnDevInst)
		{
			StringBuilder sb = new StringBuilder(1024);
			int hr = Native.CM_Get_Device_ID(dnDevInst, sb, sb.Capacity, 0);
			if (hr != 0)
				throw new Win32Exception(hr);

			Native.SP_DEVINFO_DATA devData = new Native.SP_DEVINFO_DATA
			{
				cbSize = Marshal.SizeOf(typeof (Native.SP_DEVINFO_DATA))
			};
			if (!Native.SetupDiOpenDeviceInfo(this._deviceInfoSet, sb.ToString(), IntPtr.Zero, 0, devData))
				throw new Win32Exception(Marshal.GetLastWin32Error());

			return devData;
		}

		internal string GetProperty(Native.SP_DEVINFO_DATA devData, int property, string defaultValue)
		{
			if (devData == null)
				throw new ArgumentNullException("devData");

			int propertyRegDataType;
			int requiredSize;
			int propertyBufferSize = 1024;

			IntPtr propertyBuffer = Marshal.AllocHGlobal(propertyBufferSize);
			if (!Native.SetupDiGetDeviceRegistryProperty(this._deviceInfoSet,
				devData,
				property,
				out propertyRegDataType,
				propertyBuffer,
				propertyBufferSize,
				out requiredSize))
			{
				Marshal.FreeHGlobal(propertyBuffer);
				int error = Marshal.GetLastWin32Error();
				if (error != Native.ERROR_INVALID_DATA)
					throw new Win32Exception(error);
				return defaultValue;
			}

			string value = Marshal.PtrToStringAuto(propertyBuffer);
			Marshal.FreeHGlobal(propertyBuffer);
			return value;
		}

		internal int GetProperty(Native.SP_DEVINFO_DATA devData, int property, int defaultValue)
		{
			if (devData == null)
				throw new ArgumentNullException("devData");

			int propertyRegDataType;
			int requiredSize;
			int propertyBufferSize = Marshal.SizeOf(typeof (int));

			IntPtr propertyBuffer = Marshal.AllocHGlobal(propertyBufferSize);
			if (!Native.SetupDiGetDeviceRegistryProperty(this._deviceInfoSet,
				devData,
				property,
				out propertyRegDataType,
				propertyBuffer,
				propertyBufferSize,
				out requiredSize))
			{
				Marshal.FreeHGlobal(propertyBuffer);
				int error = Marshal.GetLastWin32Error();
				if (error != Native.ERROR_INVALID_DATA)
					throw new Win32Exception(error);
				return defaultValue;
			}

			int value = (int) Marshal.PtrToStructure(propertyBuffer, typeof (int));
			Marshal.FreeHGlobal(propertyBuffer);
			return value;
		}

		internal Guid GetProperty(Native.SP_DEVINFO_DATA devData, int property, Guid defaultValue)
		{
			if (devData == null)
				throw new ArgumentNullException("devData");

			int propertyRegDataType;
			int requiredSize;
			int propertyBufferSize = Marshal.SizeOf(typeof (Guid));

			IntPtr propertyBuffer = Marshal.AllocHGlobal(propertyBufferSize);
			if (Native.SetupDiGetDeviceRegistryProperty(this._deviceInfoSet,
				devData,
				property,
				out propertyRegDataType,
				propertyBuffer,
				propertyBufferSize,
				out requiredSize))
			{
				Guid value = (Guid) Marshal.PtrToStructure(propertyBuffer, typeof (Guid));
				Marshal.FreeHGlobal(propertyBuffer);
				return value;
			}

			Marshal.FreeHGlobal(propertyBuffer);
			int error = Marshal.GetLastWin32Error();
			if (error != Native.ERROR_INVALID_DATA)
				throw new Win32Exception(error);
			return defaultValue;
		}
	}
}