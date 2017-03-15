using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveInfo;

namespace LAB1.DrivesInfo
{
	public class LAB1UIService
	{
		private readonly DriveManagement _driveManagement;

		public LAB1UIService()
		{
			this._driveManagement = new DriveManagement();
		}

		public List<DriveModel> GetDrivesInfo()
		{
			return this._driveManagement.GetDriveModels();
		}
	}
}
