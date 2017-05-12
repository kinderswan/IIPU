using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace DriveInfo
{
	public class DriveManagement
	{
		private readonly List<DriveModel> _driveModels;

		public DriveManagement()
		{
			this._driveModels = new List<DriveModel>();
		    var x = this.GetAtaStrings();
		}

		public List<DriveModel>  GetDriveModels()
		{
			this.ManageObject();
			return this._driveModels;
		}

		private void ManageObject()
		{
			ManagementObjectSearcher driveQuery = new ManagementObjectSearcher("select * from Win32_DiskDrive");
			foreach (ManagementObject drive in driveQuery.Get().Cast<ManagementObject>())
			{
				this.FillDriveInfo(drive);
				this.ManageObject(drive);
			}
		}

		private void ManageObject(ManagementObject drive)
		{
			string partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", drive.Path.RelativePath);
			ManagementObjectSearcher partitionQuery = new ManagementObjectSearcher(partitionQueryText);
			foreach (ManagementObject partition in partitionQuery.Get().Cast<ManagementObject>())
			{
				this.ManageObject(drive, partition);
			}
		}

		private void ManageObject(ManagementObject drive, ManagementObject partition)
		{
			string logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", partition.Path.RelativePath);
			ManagementObjectSearcher logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText);
			foreach (ManagementObject logicalDrive in logicalDriveQuery.Get().Cast<ManagementObject>())
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
			var model = new DriveModel()
			{
				Model = Convert.ToString(drive.Properties["Model"].Value),
				SerialNumber = Convert.ToString(drive.Properties["SerialNumber"].Value),
				MemoryCapabilities = (string[]) drive.Properties["CapabilityDescriptions"].Value,
				Partitions = new List<DrivePartitionModel>()
			};
			if (Convert.ToString(drive.Properties["MediaType"].Value) != "Removable Media")
			{
				model.DMAChannel = this.GetDMAChannel();
				model.Protocol = this.GetAccessProtocol();
			    model.Atas = this.ParseAtaStrings();
			}
			this._driveModels.Add(model);
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

		private string GetDMAChannel()
		{
			WqlObjectQuery q = new WqlObjectQuery("SELECT * FROM Win32_DMAChannel");
			ManagementObjectSearcher res = new ManagementObjectSearcher(q);
			var x = res.Get();
			string caption = string.Empty;
			foreach (ManagementObject o in res.Get())
			{
				caption = o["Caption"].ToString();
			}
			return caption;
		}

		private string GetAccessProtocol()
		{
			WqlObjectQuery q = new WqlObjectQuery("SELECT * FROM Win32_IDEController");
			ManagementObjectSearcher res = new ManagementObjectSearcher(q);
			var x = res.Get();
			int protocol = 0;
			foreach (ManagementObject o in res.Get())
			{
				protocol = Convert.ToInt32(o["ProtocolSupported"]);
			}
			return Protocols.GetProtocol(protocol);
		}

	    private string GetAtaStrings()
	    {
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = Path.Combine(Environment.CurrentDirectory, "TransferMode");

            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output;
	    }

	    private List<string> ParseAtaStrings()
	    {
	        return this.GetAtaStrings().Split(new string[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();
	    }
	}
}
