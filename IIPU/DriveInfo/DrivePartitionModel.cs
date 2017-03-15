using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveInfo
{
	public class DrivePartitionModel
	{

		public string Letter { get; set; }

		public ulong AllMemory { get; set; }

		public ulong FreeMemory { get; set; }

		public ulong TakenMemory { get; set; }
	}
}
