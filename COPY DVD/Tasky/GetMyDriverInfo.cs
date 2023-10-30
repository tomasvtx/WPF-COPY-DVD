using COPY_DVD.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace COPY_DVD.Tasky
{
	internal class GetMyDriverInfo
	{
		public async static Task<MyDriversItemViewModel> GenMydriverInfo(DriveInfo item)
		{
			string DriveFormat = "Neznámé";
			DriveType DriveType = DriveType.Unknown;
			string Name = "Neznámé";
			bool? IsReady = null;
			DirectoryInfo RootDirectory = null;
			long? TotalFreeSpace = null;
			long? TotalSize = null;
			long? AvailableFreeSpace = null;
			string VolumeLabel = "Neznámé";

			await Task.Run(() =>
			{
				try
				{
					DriveFormat = item.DriveFormat;
				}
				catch { }
				try
				{
					DriveType = item.DriveType;
				}
				catch { }
				try
				{
					Name = item.Name;
				}
				catch { }
				try
				{
					IsReady = item.IsReady;
				}
				catch { }
				try
				{
					RootDirectory = item.RootDirectory;
				}
				catch { }
				try
				{
					TotalFreeSpace = item.TotalFreeSpace;
				}
				catch { }
				try
				{
					TotalSize = item.TotalSize;
				}
				catch { }
				try
				{
					AvailableFreeSpace = item.AvailableFreeSpace;
				}
				catch { }
				try
				{
					VolumeLabel = item.VolumeLabel;
				}
				catch { }
			});

			return await Task.FromResult(new MyDriversItemViewModel(new MyDriveInfo
			{
				AvailableFreeSpace = AvailableFreeSpace,
				DriveFormat = DriveFormat,
				DriveType = DriveType,
				IsReady = IsReady,
				Name = Name,
				RootDirectory = RootDirectory,
				TotalFreeSpace = TotalFreeSpace,
				TotalSize = TotalSize,
				VolumeLabel = VolumeLabel
			}));
		}
	}
}
