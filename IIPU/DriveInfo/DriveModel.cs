using System.Collections.Generic;

namespace DriveInfo
{
	public class DriveModel
	{
		public string Model { get; set; }

		public string SerialNumber { get; set; }

		public string DMAChannel { get; set; }

		public string Protocol { get; set; }

		public string[] MemoryCapabilities { get; set; }

		public List<DrivePartitionModel> Partitions { get; set; }
	}
}
