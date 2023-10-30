using COPY_DVD.ViewModel;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace COPY_DVD
{
	/// <summary>
	/// Interakční logika pro MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// auto update state of optical drivers
		/// </summary>
		DispatcherTimer Check_driveState = new DispatcherTimer();

		/// <summary>
		/// Update time
		/// </summary>
		DispatcherTimer Time = new DispatcherTimer();
		private bool exitApp = false;

		public MainWindow()
		{
			InitializeComponent();

			///auto update state of optical drivers
			Check_driveState.Tick += Check_driveState_Tick;
			Check_driveState.Start();

			///Update time
			Time.Tick += Time_Tick;
			Time.Interval = TimeSpan.FromMilliseconds(100);
			Time.Start();

			App.ViewModel.Title = "DVD COPY";

			Closing += MainWindow_Closing;
		}

		/// <summary>
		/// Closing event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;

			close.Visibility = Visibility.Visible;

			await Task.Delay(100);

			if (App.ViewModel.Copy_ViewModel.SemaphoreSlim.CurrentCount == 0) { 
			App._Dispatcher.Invoke(() => App.ViewModel.StatusViewModel.StopContent = "Zastavuji", DispatcherPriority.Background);
			App.ViewModel.Copy_ViewModel.CancellationTokenSource.Cancel();

			await Task.Run(async () =>
			{
				while (App.ViewModel.Copy_ViewModel.SemaphoreSlim.CurrentCount == 0)
				{
					await Task.Delay(10);
				}
			});
			}
			exitApp = true;
		}

		/// <summary>
		/// Update time
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Time_Tick(object sender, EventArgs e)
		{
			if (App.ViewModel.Scroolingdown == true)
			{
				prubeh.ScrollIntoView(App.ViewModel?.Copy_ViewModel?.Models?.LastOrDefault());
				chyby.ScrollIntoView(App.ViewModel?.Copy_ViewModel?.Errors?.LastOrDefault());
			}
			App.ViewModel.Datumcas = DateTime.Now;
		}

		/// <summary>
		/// auto update state of optical drivers
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Check_driveState_Tick(object sender, EventArgs e)
		{
			Check_driveState.Stop();

			App.ViewModel.StatusViewModel.Cursor = System.Windows.Input.Cursors.ArrowCD;

			///task for finding DVD drivers
			await Task.Run(() =>
			{
				Tasky.Check_drivers_state check_Drivers_State = new Tasky.Check_drivers_state();
				check_Drivers_State.Thread.Start();
				check_Drivers_State.Thread.Join();
			});

			App.ViewModel.StatusViewModel.Cursor = System.Windows.Input.Cursors.Arrow;
			await Task.Delay(100);

			await Task.Run(() =>
			{
				Tasky.CheckDriverState checkDriverState = new Tasky.CheckDriverState();
				checkDriverState.Thread.Start();
				checkDriverState.Thread.Join();
			});

			App.ViewModel.StatusViewModel.Cursor = System.Windows.Input.Cursors.Arrow;
			await Task.Delay(100);

			if (exitApp)
			{
				App.Current.Shutdown();
			}
			else
			{
				Check_driveState.Start();
			}
		}


		private async void Copy_folder(object sender, RoutedEventArgs e)
		{
			await Task.Run(() =>
			{
				var Drive = App.ViewModel.SelectedDriver;
			Repeat:
				if (Drive != null)
				{
					if (Drive.IsReady == false)
					{
						MessageBoxResult result = App._Dispatcher.Invoke(() => System.Windows.MessageBox.Show("Disk nebyl vložen\n\nOpakovat?", "Není zjišten disk", MessageBoxButton.YesNo, MessageBoxImage.Information), DispatcherPriority.Background);

						if (result == MessageBoxResult.Yes)
						{
							goto Repeat;
						}

						return;
					}

					FolderBrowserDialog dlgFolder = new FolderBrowserDialog();
					Thread thread = new Thread(async () =>
					{
						dlgFolder.SelectedPath = App.ViewModel.LastPath;
						var resultdial = await Task.Run(() => App._Dispatcher.Invoke(() => dlgFolder.ShowDialog(), DispatcherPriority.Background));

						if (resultdial == System.Windows.Forms.DialogResult.OK)
						{
							if (Drive.VolumeLabel == "Neznámé" || Drive.VolumeLabel == "")
							{
								App._Dispatcher.Invoke(() => System.Windows.MessageBox.Show("Název disku nebyl nalezen", "Není jmenovka disku", MessageBoxButton.OK, MessageBoxImage.Information), DispatcherPriority.Background);

								App._Dispatcher.Invoke(() => App.ViewModel.LastPath = $"{dlgFolder.SelectedPath}\\VOLUME", DispatcherPriority.Background);
							}
							else
							{
								App._Dispatcher.Invoke(() => App.ViewModel.LastPath = $"{dlgFolder.SelectedPath}\\{Drive.VolumeLabel}", DispatcherPriority.Background);
							}
						}
					});
					thread.Start();
					thread.Join();

				}
				else
				{
					App._Dispatcher.Invoke(() => System.Windows.MessageBox.Show("Vyberte optickou jednotku...", "Není vybrána optická jednotka", MessageBoxButton.OK, MessageBoxImage.Information), DispatcherPriority.Background);
				}
			});
		}

		private void Stop_task(object sender, RoutedEventArgs e)
		{
			App._Dispatcher.Invoke(() => App.ViewModel.StatusViewModel.StopContent = "Zastavuji", DispatcherPriority.Background);
			App.ViewModel.Copy_ViewModel.CancellationTokenSource.Cancel();
		}
	}
}
