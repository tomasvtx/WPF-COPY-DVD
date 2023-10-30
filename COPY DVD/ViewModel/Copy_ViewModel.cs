using COPY_DVD.ViewModel;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace COPY_DVD.Tasky
{
	public class Copy_ViewModel : ViewModelBase
	{

		public CancellationTokenSource CancellationTokenSource;
		public SemaphoreSlim SemaphoreSlim;

		private ObservableCollection<string> models;
		private ObservableCollection<string> errors;
		private ObservableCollection<FileList> files;
		private byte repeat;
		private int fileProcessed;
		private double procenta;
		private ObservableCollection<FolderScan> folder;

		public Copy_ViewModel()
		{
			SemaphoreSlim = new SemaphoreSlim(1, 1);

			Folders = new ObservableCollection<FolderScan>();
			Models = new ObservableCollection<string>();
			Files = new ObservableCollection<FileList>();
			Errors = new ObservableCollection<string>();

			CancellationTokenSource = new CancellationTokenSource();
			CopyCommand = new DelegateCommand(async () => await Task_(), CanExecuteCopy);
		}

		public ObservableCollection<FolderScan> Folders
		{
			get
			{
				return folder;
			}
			set
			{
				folder = value;
				UpdateUI();
			}
		}

		public ObservableCollection<FileList> Files
		{
			get
			{
				return files;
			}
			set
			{
				files = value;
				UpdateUI();
			}
		}

		public ObservableCollection<string> Models
		{
			get
			{
				return models;
			}
			set
			{
				models = value;
				UpdateUI();
			}
		}

		public ObservableCollection<string> Errors
		{
			get
			{
				return errors;
			}
			set
			{
				errors = value;
				UpdateUI();
			}
		}

		public byte Repeats
		{
			get => repeat;
			set
			{
				repeat = value;
				UpdateUI();
			}
		}

		public int FileProcessed
		{
			get => fileProcessed;
			set
			{
				fileProcessed = value;
				UpdateUI();
			}
		}
		public double Procenta
		{
			get => procenta;
			set
			{
				procenta = value;
				UpdateUI();
			}
		}

		private bool CanExecuteCopy()
		{
			return true;
		}

		public DelegateCommand CopyCommand { get; set; }

		public async Task Task_()
		{
			if (SemaphoreSlim.CurrentCount == 0)
			{
				App._Dispatcher.Invoke(() => MessageBox.Show("Nelze spustit úlohu znovu, počtejte na dokončení úlohy, nebo ji přerušte", "SemaphoreSlim"), DispatcherPriority.Normal);
			}

			await SemaphoreSlim.WaitAsync();
			
			MyDriversItemViewModel Driver = App.ViewModel.SelectedDriver;

			if (Driver == null || Driver.IsReady == false)
			{
				App._Dispatcher.Invoke(() => MessageBox.Show("Jednotka není vybrána, nebo připravena ke čtení\n"), DispatcherPriority.Normal);
				SemaphoreSlim?.Release(); return;
			}

			if (App.ViewModel.LastPath == null || App.ViewModel.LastPath.Length < 4 || !App.ViewModel.LastPath.Contains("\\"))
			{
				App._Dispatcher.Invoke(() => System.Windows.MessageBox.Show("cílová složka je neplatná\n" + App.ViewModel.LastPath), DispatcherPriority.Normal);
				SemaphoreSlim?.Release();
				return;
			}

			string folder = $"{App.ViewModel.LastPath}\\";

			CancellationTokenSource = new CancellationTokenSource();

			App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Files = new System.Collections.ObjectModel.ObservableCollection<FileList>(), DispatcherPriority.Normal);
			App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Errors = new System.Collections.ObjectModel.ObservableCollection<string>(), DispatcherPriority.Normal);
			App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models = new System.Collections.ObjectModel.ObservableCollection<string>(), DispatcherPriority.Normal);

			try
			{
				await Tasky.Copy.PočítatSoubory(folder, Driver);

				await Task.Run(() =>
				{
					try
					{
						Tasky.Copy.CopyDirectory(folder, CancellationTokenSource.Token, Driver);
						App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - úloha kopírování dokončena"), DispatcherPriority.Normal);
						App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - úloha kopírování dokončena"), DispatcherPriority.Normal);
					}
					catch (Exception s)

					{
						System.Windows.MessageBox.Show(s.Message);
					}
				}
					);
			}
			catch (Exception ss)
			{
				System.Windows.MessageBox.Show(ss.Message);
			}

			App._Dispatcher.Invoke(() => App.ViewModel.StatusViewModel.StopContent = "Přerušit", DispatcherPriority.Background);
			App._Dispatcher.Invoke(() => App.ViewModel.StatusViewModel.CopyStatus_visibility = Visibility.Hidden, DispatcherPriority.Background);
			App._Dispatcher.Invoke(() => App.ViewModel.StatusViewModel.IsIndeterminate = true, DispatcherPriority.Background);

			SemaphoreSlim?.Release();
		}
	}
}
