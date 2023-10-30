using COPY_DVD.Tasky;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace COPY_DVD.ViewModel
{
	public class ViewModel : ViewModelBase
	{

		private ObservableCollection<MyDriversItemViewModel> drivers;
		private MyDriversItemViewModel selectedDriver;
		private DateTime datumcas;

		private string lastpath;

		private bool? overwrite;
		private bool? fileerror;
		private List<PočetOpakování> _početOpakování;
		private PočetOpakování selectedRepeat;

		private string title;
		private bool scroolingdown;

		public ViewModel()
		{
			Copy_ViewModel = new Copy_ViewModel();
			StatusViewModel = new Task_statusViewModel();

			Drivers = new ObservableCollection<MyDriversItemViewModel>();
			Overwrite = null;
			FileError = null;
			_PočetOpakování = new List<PočetOpakování>();
			_PočetOpakování.Add(new PočetOpakování { Name = "0 - při chybě přeskočit", Počet = 0 });

			for (byte i = 1; i < 50; i++)
			{
				_PočetOpakování.Add(new PočetOpakování { Name = i + " - při chybě opakovat " + i + "x", Počet = i });
			}

			SelectedRepeat = _PočetOpakování.LastOrDefault();
			Scroolingdown = true;
		}

		public Copy_ViewModel Copy_ViewModel { get; set; }
		public Task_statusViewModel StatusViewModel { get; set; }


		public ObservableCollection<MyDriversItemViewModel> Drivers
		{
			get => drivers;

			set
			{
				App._Dispatcher.Invoke(() => drivers = value, System.Windows.Threading.DispatcherPriority.Normal);
				UpdateUI();
			}
		}

		public MyDriversItemViewModel SelectedDriver
		{
			get => selectedDriver;

			set
			{
				selectedDriver = value;
				UpdateUI();
			}
		}

		public DateTime Datumcas
		{
			get => datumcas;

			set
			{
				datumcas = value;
				UpdateUI();
			}
		}

		public string LastPath
		{
			get
			{
				return lastpath;
			}
			set
			{
				lastpath = value;
				UpdateUI();
			}
		}

		public bool? Overwrite
		{
			get
			{
				return overwrite;
			}
			set
			{
				overwrite = value;
				UpdateUI();
			}
		}

		public bool? FileError
		{
			get
			{
				return fileerror;
			}
			set
			{
				fileerror = value;
				UpdateUI();
			}
		}


		public List<PočetOpakování> _PočetOpakování
		{
			get => _početOpakování;
			set
			{
				_početOpakování = value;
				UpdateUI();
			}
		}

		public PočetOpakování SelectedRepeat
		{
			get => selectedRepeat;
			set
			{
				selectedRepeat = value;
				UpdateUI();
			}
		}

		public string Title
		{
			get => title; set
			{
				App._Dispatcher.Invoke(() => title = value, System.Windows.Threading.DispatcherPriority.Normal);
				UpdateUI();
			}
		}

		public bool Scroolingdown
		{
			get => scroolingdown; set
			{
				scroolingdown = value;
				UpdateUI();
			}
		}
	}
}
