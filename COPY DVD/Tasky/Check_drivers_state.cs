using COPY_DVD.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace COPY_DVD.Tasky
{
	internal class Check_drivers_state
	{
		public Check_drivers_state()
		{
			Thread = new Thread(() =>
			{
				Find_opticalDrivers();
			});
		}

		public Thread Thread { get; }

		public static Task Find_opticalDrivers()
		{
			for (int i = 0; i < Directory.GetLogicalDrives().Length; i++)
			{
				var item = new DriveInfo(Directory.GetLogicalDrives()[i]);

				///jednotka je prodána poue když je typu CD, nebo DVD
				if (item.DriveType == DriveType.CDRom && !App.ViewModel.Drivers.Any(ee => ee.Name == item.Name))
				{
					App._Dispatcher.Invoke(async () => App.ViewModel.Drivers.Add(await Tasky.GetMyDriverInfo.GenMydriverInfo(item)), DispatcherPriority.Background);
				}
			}

			return Task.CompletedTask;
		}
	}

	public class CheckDriverState
	{
		private static MyDriversItemViewModel sel_drv;
		public CheckDriverState()
		{
			Thread = new Thread(async () =>
			{
				await Update_Optical_drive_info();
			});
		}

		public Thread Thread { get; }

		public static async Task<Task> Update_Optical_drive_info()
		{
			var SelectedDriver = App._Dispatcher.Invoke(() => App.ViewModel.SelectedDriver);

			///Zpracování proběhne poue když je nějaká jednotka vybrána
			if (SelectedDriver?.Name != null)
			{
				string _Name = SelectedDriver.Name;

				App._Dispatcher.Invoke(() => App.ViewModel.StatusViewModel.Cursor = System.Windows.Input.Cursors.ArrowCD, DispatcherPriority.Background);

				bool founded_actual = false;

				///Prohledáná jenotky v počítači avymete tu, která odpovídá zvolené jednotce
				foreach (var item in Directory.GetLogicalDrives())
				{
					var _item = new DriveInfo(item);

					if (_item.Name == _Name)
					{
						///Zjistí se akuální onfo o jednotce
						sel_drv = await Tasky.GetMyDriverInfo.GenMydriverInfo(_item);
						founded_actual = true;
					}

					///nalezené informace se uloží do MWWM
					if (App.ViewModel.Drivers.FirstOrDefault(ee => ee.RootDirectory == sel_drv?.RootDirectory) != null)
					{
						App._Dispatcher.Invoke(() => App.ViewModel.Drivers.FirstOrDefault(ee => ee.Name == sel_drv?.Name).VolumeLabel = sel_drv.VolumeLabel, DispatcherPriority.Background);
						App._Dispatcher.Invoke(() => App.ViewModel.Drivers.FirstOrDefault(ee => ee.Name == sel_drv?.Name).AvailableFreeSpace = sel_drv.AvailableFreeSpace, DispatcherPriority.Background);
						App._Dispatcher.Invoke(() => App.ViewModel.Drivers.FirstOrDefault(ee => ee.Name == sel_drv?.Name).TotalFreeSpace = sel_drv.TotalFreeSpace, DispatcherPriority.Background);
						App._Dispatcher.Invoke(() => App.ViewModel.Drivers.FirstOrDefault(ee => ee.Name == sel_drv?.Name).DriveFormat = sel_drv.DriveFormat, DispatcherPriority.Background);
						App._Dispatcher.Invoke(() => App.ViewModel.Drivers.FirstOrDefault(ee => ee.Name == sel_drv?.Name).DriveType = sel_drv.DriveType, DispatcherPriority.Background);
						App._Dispatcher.Invoke(() => App.ViewModel.Drivers.FirstOrDefault(ee => ee.Name == sel_drv?.Name).RootDirectory = sel_drv.RootDirectory, DispatcherPriority.Background);
						App._Dispatcher.Invoke(() => App.ViewModel.Drivers.FirstOrDefault(ee => ee.Name == sel_drv?.Name).Name = sel_drv.Name, DispatcherPriority.Background);
						App._Dispatcher.Invoke(() => App.ViewModel.Drivers.FirstOrDefault(ee => ee.Name == sel_drv?.Name).IsReady = sel_drv.IsReady, DispatcherPriority.Background);
						App._Dispatcher.Invoke(() => App.ViewModel.Drivers.FirstOrDefault(ee => ee.Name == sel_drv?.Name).TotalSize = sel_drv.TotalSize, DispatcherPriority.Background);
					}

					App._Dispatcher.Invoke(() => App.ViewModel.SelectedDriver = sel_drv, DispatcherPriority.Background);
				}
				if (founded_actual == false)
				{
					App._Dispatcher.Invoke(() => App.ViewModel.SelectedDriver = null, DispatcherPriority.Background);
				}
			}

			return Task.CompletedTask;
		}
	}
}
