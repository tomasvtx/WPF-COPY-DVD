using System.IO;

namespace COPY_DVD.ViewModel
{
	public class MyDriveInfo
	{
		public string Name { get; set; }
		public DriveType DriveType { get; set; }
	public string DriveFormat { get; set; }
		public bool? IsReady { get; set; }
		public long? AvailableFreeSpace { get; set; }
		public long? TotalFreeSpace { get; set; }
		public long? TotalSize { get; set; }
		public DirectoryInfo RootDirectory { get; set; }
		public string VolumeLabel { get; set; }
	}
}