using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace DriveInfo
{
	public class DriveManagement
	{
		private List<DriveModel> _driveModels;

		public DriveManagement()
		{
			this._driveModels = new List<DriveModel>();
		}

		public List<DriveModel>  GetDriveModels()
		{
			this.ManageObject();
			return this._driveModels;
		}

		private void ManageObject()
		{
			ManagementObjectSearcher driveQuery = new ManagementObjectSearcher("select * from Win32_DiskDrive");
			foreach (ManagementObject drive in driveQuery.Get())
			{
				this.FillDriveInfo(drive);
				this.ManageObject(drive);
			}
		}

		private void ManageObject(ManagementObject drive)
		{
			string partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", drive.Path.RelativePath);
			ManagementObjectSearcher partitionQuery = new ManagementObjectSearcher(partitionQueryText);
			foreach (ManagementObject partition in partitionQuery.Get())
			{
				this.ManageObject(drive, partition);
			}
		}

		private void ManageObject(ManagementObject drive, ManagementObject partition)
		{
			string logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", partition.Path.RelativePath);
			ManagementObjectSearcher logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText);
			foreach (ManagementObject logicalDrive in logicalDriveQuery.Get())
			{
				this.ManageObject(drive, partition, logicalDrive);
			}
		}

		private void ManageObject(ManagementObject drive, ManagementObject partition, ManagementObject logicalDrive)
		{
			this.FillLogicalDrivesInfo(drive, logicalDrive);
		}

		private void FillDriveInfo(ManagementObject drive)
		{
			this._driveModels.Add(new DriveModel()
			{
				Model = Convert.ToString(drive.Properties["Model"].Value),
				SerialNumber = Convert.ToString(drive.Properties["SerialNumber"].Value),
				MemoryCapabilities = (string[])drive.Properties["CapabilityDescriptions"].Value,
				Partitions = new List<DrivePartitionModel>(),
				AtaStandard = Convert.ToString(drive.Properties["InterfaceType"].Value),
			});
		}

		private void FillLogicalDrivesInfo(ManagementObject drive, ManagementObject logicalDrive)
		{
			DriveModel driveModel = this._driveModels.FirstOrDefault(x => x.Model == Convert.ToString(drive.Properties["Model"].Value));
			if (driveModel == null)
			{
				return;
			}

			DrivePartitionModel model = new DrivePartitionModel()
			{
				AllMemory = Convert.ToUInt64(logicalDrive.Properties["Size"].Value),
				FreeMemory = Convert.ToUInt64(logicalDrive.Properties["FreeSpace"].Value),
				Letter = Convert.ToString(logicalDrive.Properties["Name"].Value)
			};

			model.TakenMemory = model.AllMemory - model.FreeMemory;
			driveModel.Partitions.Add(model);
		}
	}
}
