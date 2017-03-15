using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveInfo
{
	public class DriveModel
	{
		public string Model { get; set; }

		public string SerialNumber { get; set; }

		public string AtaStandard { get; set; }

		public string[] MemoryCapabilities { get; set; }

		public List<DrivePartitionModel> Partitions { get; set; }
	}
}
