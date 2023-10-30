using System;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace COPY_DVD.ViewModel
{
	public class FileList
	{
		public FileInfo FileInfo { get; set; }
		public bool FolderCreated { get; set; }
		public bool FileCopy { get; set; }
		public TimeSpan FileCopy_dur { get; set; }
	}

	public class FolderScan
	{
		public string FolderName { get; set; }
		public bool ScanSoubdirectories { get; set; }
		public bool? FolderContain_any_folder { get; set; }
	}
}