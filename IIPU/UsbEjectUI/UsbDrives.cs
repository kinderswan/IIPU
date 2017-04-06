using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using USBEject;
using DriveInfo;

namespace UsbEjectUI
{
	class UsbDrives
	{
		private List<string> UsbDrivesList { get; set; }
		private VolumeDeviceClass volumeDeviceClass;
		private DriveManagement driveManagement;

		public UsbDrives()
		{
			this.driveManagement = new DriveManagement();
		}

		public List<string> GetUsbDrivesList()
		{
			this.volumeDeviceClass = new VolumeDeviceClass();
			this.FillUsbDrivesList();
			return this.UsbDrivesList;
		}

		public void EjectDrive(string name)
		{
			foreach (var volume in this.volumeDeviceClass.Devices.Where(volume => ((Volume)volume).LogicalDrive == name))
			{
				volume.Eject(false);
			}
		}

		private DrivePartitionModel GetUsbPartitionData(string name)
		{
			var x = this.driveManagement.GetDriveModels();
			DrivePartitionModel dModel = null;
			foreach (DrivePartitionModel drivePartitionModel in x.Select(model => model.Partitions.FirstOrDefault(y => y.Letter == name)).Where(drivePartitionModel => drivePartitionModel != null))
			{
				dModel = drivePartitionModel;
			}

			return dModel;
		}

		private void FillUsbDrivesList()
		{
			this.UsbDrivesList = this.volumeDeviceClass.Devices.Where(volume => volume.IsUsb).Select(volume => ((Volume)volume).LogicalDrive).ToList();
		}

		public string CreateUsbFieldData(string usb)
		{
			var info = this.GetUsbPartitionData(usb);
			var label = this.GetVolumeLabel(usb);
			return string.Format("Name: {0} VolumeLabel: {1} TotalSpace: {2} FreeSpace: {3} TakenSpace: {4}", usb, label, info.AllMemory, info.FreeMemory,
				info.TakenMemory);

		}

		private string GetVolumeLabel(string name)
		{
			var drives = System.IO.DriveInfo.GetDrives();
			return drives.FirstOrDefault(x=>x.Name == name + @"\").VolumeLabel;
		}
	}
}
