using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DriveInfo;

namespace LAB1.DrivesInfo
{
	public partial class Lab1UI : Form
	{
		private readonly LAB1UIService _lab1UiService;

		private readonly List<DriveModel> _driveModels; 

		public Lab1UI()
		{
			this.InitializeComponent();
			this._lab1UiService = new LAB1UIService();
			this._driveModels = this._lab1UiService.GetDrivesInfo();
		}

		private void getInfoButton_Click(object sender, EventArgs e)
		{
			this.FillDrivesModelListbox();
		}

		private void FillDrivesModelListbox()
		{
			List<string> items = this._driveModels.Select(x => x.Model).ToList();
			foreach (string item in items)
			{
				this.driveModelsListBox.Items.Add("Model: " + item);
			}
		}

		private void driveModelsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.driveModelsInfoListBox.Items.Clear();
			string selectedItem = ((ListBox) sender).SelectedItem.ToString().Replace("Model: ", "");
			DriveModel driveInfo = this._driveModels.FirstOrDefault(x => x.Model == selectedItem);
			if (driveInfo == null)
			{
				return;
			}

			this.driveModelsInfoListBox.Items.Add("DMAChannel: " + driveInfo.DMAChannel);
			this.driveModelsInfoListBox.Items.Add("Protocol: " + driveInfo.Protocol);
			this.driveModelsInfoListBox.Items.Add("SerialNumber: " + driveInfo.SerialNumber);
			foreach (string memoryCapability in driveInfo.MemoryCapabilities)
			{
				this.driveModelsInfoListBox.Items.Add("Memory Capability: " + memoryCapability);
			}

			this.FillLogicalDrivesModelListbox(driveInfo);
		}

		private void FillLogicalDrivesModelListbox(DriveModel selectedDrive)
		{
			this.logicalDriveModelsListBox.Items.Clear();
			foreach (DrivePartitionModel partition in selectedDrive.Partitions)
			{
				this.logicalDriveModelsListBox.Items.Add("Name: " + partition.Letter);
			}
		}

		private void logicalDriveModelsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedItem = ((ListBox) sender).SelectedItem.ToString().Replace("Name: ", "");
			DrivePartitionModel selected = null;

			foreach (DrivePartitionModel part in this._driveModels.SelectMany(drive => drive.Partitions.Where(part => part.Letter == selectedItem)))
			{
				selected = part;
			}

			this.logicalDrivesInfoListBox.Items.Clear();
			if (selected == null)
			{
				return;
			}
			this.logicalDrivesInfoListBox.Items.Add("Total Memory: " + selected.AllMemory);
			this.logicalDrivesInfoListBox.Items.Add("Free Memory: " + selected.FreeMemory);
			this.logicalDrivesInfoListBox.Items.Add("Taken Memory: " + selected.TakenMemory);
		}
	}
}
