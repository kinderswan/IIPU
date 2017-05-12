using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace USBEject
{
	[TypeConverter(typeof (ExpandableObjectConverter))]
	public class Device : IComparable
	{
		private readonly DeviceClass _deviceClass;
		private readonly Native.SP_DEVINFO_DATA _deviceInfoData;
		private readonly int _disknum;
		private readonly int _index;
		private readonly string _path;
		private DeviceCapabilities _capabilities = DeviceCapabilities.Unknown;
		private string _class;
		private string _classGuid;
		private string _description;
		private string _friendlyName;
		private Device _parent;
		private List<Device> _removableDevices;

		internal Device(DeviceClass deviceClass, Native.SP_DEVINFO_DATA deviceInfoData, string path, int index, int disknum = -1)
		{
			if (deviceClass == null)
				throw new ArgumentNullException("deviceClass");

			if (deviceInfoData == null)
				throw new ArgumentNullException("deviceInfoData");

			this._deviceClass = deviceClass;
			this._path = path;
			this._deviceInfoData = deviceInfoData;
			this._index = index;
			this._disknum = disknum;
		}

		public int Index
		{
			get { return this._index; }
		}

		[Browsable(false)]
		public DeviceClass DeviceClass
		{
			get { return this._deviceClass; }
		}

		public string Path
		{
			get { return this._path; }
		}

		public int DiskNumber
		{
			get { return this._disknum; }
		}

		public int InstanceHandle
		{
			get { return this._deviceInfoData.deviceInstance; }
		}

		public string Class
		{
			get { return this._class ?? (this._class = this.GetDeviceProperty(Native.SPDRP_CLASS)); }
		}

		public string ClassGuid
		{
			get { return this._classGuid ?? (this._classGuid = this.GetDeviceProperty(Native.SPDRP_CLASSGUID)); }
		}

		public string Description
		{
			get { return this._description ?? (this._description = this.GetDeviceProperty(Native.SPDRP_DEVICEDESC)); }
		}

		public string FriendlyName
		{
			get { return this._friendlyName ?? (this._friendlyName = this.GetDeviceProperty(Native.SPDRP_FRIENDLYNAME)); }
		}

		public DeviceCapabilities Capabilities
		{
			get
			{
				if (this._capabilities == DeviceCapabilities.Unknown)
				{
					this._capabilities = (DeviceCapabilities) this._deviceClass.GetProperty(this._deviceInfoData, Native.SPDRP_CAPABILITIES, 0);
				}
				return this._capabilities;
			}
		}

		public virtual bool IsUsb
		{
			get
			{
				if (this.Class == "USB")
				{
					return true;
				}

				return (this.Parent != null) && this.Parent.IsUsb;
			}
		}

		public Device Parent
		{
			get
			{
				if (this._parent == null)
				{
					int parentDevInst = 0;
					int hr = Native.CM_Get_Parent(ref parentDevInst, this._deviceInfoData.deviceInstance, 0);
					if (hr == 0)
					{
						this._parent = new Device(this._deviceClass, this._deviceClass.GetInfo(parentDevInst), null, -1);
					}
				}
				return this._parent;
			}
		}

		public virtual List<Device> RemovableDevices
		{
			get
			{
				if (this._removableDevices == null)
				{
					this._removableDevices = new List<Device>();

					if ((this.Capabilities & DeviceCapabilities.Removable) != 0)
					{
						this._removableDevices.Add(this);
					}
					else
					{
						if (this.Parent == null)
						{
							return this._removableDevices;
						}

						foreach (Device device in this.Parent.RemovableDevices)
						{
							this._removableDevices.Add(device);
						}
					}
				}
				return this._removableDevices;
			}
		}

		public virtual int CompareTo(object obj)
		{
			Device device = obj as Device;
			if (device == null)
				throw new ArgumentException();

			return this.Index.CompareTo(device.Index);
		}

		public string Eject(bool allowUI)
		{
			foreach (Device device in this.RemovableDevices)
			{
				if (allowUI)
				{
					Native.CM_Request_Device_Eject_NoUi(device.InstanceHandle, IntPtr.Zero, null, 0, 0);
				}
				else
				{
					StringBuilder sb = new StringBuilder(1024);

					Native.PNP_VETO_TYPE veto;
					int hr = Native.CM_Request_Device_Eject(device.InstanceHandle, out veto, sb, sb.Capacity, 0);
					if (hr != 0)
						throw new Win32Exception(hr);

					if (veto != Native.PNP_VETO_TYPE.Ok)
						return veto.ToString();
				}
			}
			return null;
		}

		private string GetDeviceProperty(int SPDRP)
		{
			return this._deviceClass.GetProperty(this._deviceInfoData, SPDRP, null);
		}
	}
}