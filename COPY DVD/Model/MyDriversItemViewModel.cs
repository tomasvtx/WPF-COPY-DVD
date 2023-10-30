using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COPY_DVD.ViewModel
{
	public class MyDriversItemViewModel:ViewModelBase
	{
		private MyDriveInfo _myDriveInfo;

		public MyDriversItemViewModel(MyDriveInfo myDriveInfo)
		{
			_myDriveInfo = myDriveInfo;
		}	

		public string Name
		{
			get
			{
				return _myDriveInfo.Name;
			}
			set
			{
				_myDriveInfo.Name = value;
			}
		}

		public string DriveFormat
		{
			get
			{
				return _myDriveInfo.DriveFormat;
			}
			set
			{
				_myDriveInfo.DriveFormat = value;
			}
		}

		public DriveType DriveType
		{
			get
			{
				return _myDriveInfo.DriveType;
			}
			set
			{
				_myDriveInfo.DriveType = value;
			}
		}

		public string VolumeLabel
		{
			get
			{
				return _myDriveInfo.VolumeLabel;
			}
			set
			{
				_myDriveInfo.VolumeLabel = value;
			}
		}

		public long? TotalSize
		{
			get
			{
				return _myDriveInfo.TotalSize;
			}
			set
			{
				_myDriveInfo.TotalSize = value;
			}
		}

		public long? TotalFreeSpace
		{
			get
			{
				return _myDriveInfo.TotalFreeSpace;
			}
			set
			{
				_myDriveInfo.TotalFreeSpace = value;
			}
		}
		
		public long? AvailableFreeSpace
		{
			get
			{
				return _myDriveInfo.AvailableFreeSpace;
			}
			set
			{
				_myDriveInfo.AvailableFreeSpace = value;
			}
		}

		public bool? IsReady
		{
			get
			{
				return _myDriveInfo.IsReady;
			}
			set
			{
				_myDriveInfo.IsReady = value;
			}
		}

		public DirectoryInfo RootDirectory
		{
			get
			{
				return _myDriveInfo.RootDirectory;
			}
			set
			{
				_myDriveInfo.RootDirectory = value;
			}
		}
	}
}
