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
