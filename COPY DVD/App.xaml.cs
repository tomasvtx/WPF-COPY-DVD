using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace COPY_DVD
{
	/// <summary>
	/// Interakční logika pro App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static Dispatcher _Dispatcher;

		public static ViewModel.ViewModel ViewModel { get; set; }

		protected async override void OnSessionEnding(SessionEndingCancelEventArgs e)
		{
			e.Cancel = true;

			await Task.Delay(100);

			if (App.ViewModel.Copy_ViewModel.SemaphoreSlim.CurrentCount == 0)
			{
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

			e.Cancel = false;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			_Dispatcher = Dispatcher.CurrentDispatcher;

			ViewModel = new ViewModel.ViewModel();

			MainWindow mainWindow = new MainWindow();
			mainWindow.DataContext = ViewModel;
			mainWindow.Show();
		}
	}
}
