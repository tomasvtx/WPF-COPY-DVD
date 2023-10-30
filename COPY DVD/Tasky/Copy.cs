using COPY_DVD.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace COPY_DVD.Tasky
{
	internal class Copy
	{
		public static async Task PočítatSoubory(string folder, MyDriversItemViewModel driver)
		{
			await Task.Run(async () =>
			{
				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - Počítám soubory na DVD"), DispatcherPriority.Normal);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - Počítám soubory na DVD"), DispatcherPriority.Normal);

				if (Directory.Exists(folder))
				{
					var quest = App._Dispatcher.Invoke(() => System.Windows.MessageBox.Show($"Složka {folder} již existuje, chcete spustit úlohu kopírování?", "Složka již existuje", MessageBoxButton.YesNo, MessageBoxImage.Question));

					if (quest == MessageBoxResult.No)
					{
						App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - Uloha počítaní byla zrušena uživatelem"), DispatcherPriority.Normal);
						App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - Uloha počítaní byla zrušena uživatelem"), DispatcherPriority.Normal);
						return Task.FromException(new Exception("Zrušeno uživatelem"));
					}
				}

				App._Dispatcher.Invoke(() => App.ViewModel.StatusViewModel.IsIndeterminate = true, DispatcherPriority.Background);
				App._Dispatcher.Invoke(() => App.ViewModel.StatusViewModel.CopyStatus_visibility = Visibility.Visible, DispatcherPriority.Background);

				try
				{
					await Task.Run(async () =>
					{
						await getFolders(driver.Name);
					});

					if (!(App.ViewModel.Copy_ViewModel.Files.Count > 0))
					{
						App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - Uloha počítaní byla zrušena - nebyli nalezeny soubory"), DispatcherPriority.Normal);
						App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - Uloha počítaní byla zrušena - nebyli nalezeny soubory"), DispatcherPriority.Normal);

						return Task.FromException(new Exception("nebyli nalezeny soubory"));
					}

					App._Dispatcher.Invoke(() => App.ViewModel.Title = ($"DVD COPY - počet souborů na DVD je: {App.ViewModel.Copy_ViewModel.Files.Count}"), DispatcherPriority.Normal);
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add($"DVD COPY - počet souborů na DVD je: {App.ViewModel.Copy_ViewModel.Files.Count}"), DispatcherPriority.Normal);

					return Task.CompletedTask;
				}
				catch (Exception ex)
				{
					App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - Uloha počítaní byla zrušena - chyba " + ex.Message), DispatcherPriority.Normal);
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - Uloha počítaní byla zrušena - chyba" + ex.Message), DispatcherPriority.Normal);

					return Task.FromException(ex);
				}
			});
		}


		private async static Task getFolders(string sDir)
		{
			await Task.Run(() => getFoldersbyListAsync(sDir));

			await Task.Run(async () =>
			{
				while (true)
				{
					var Search = App.ViewModel.Copy_ViewModel.Folders.Where(ee => ee.ScanSoubdirectories == false).Where(rr => rr.FolderContain_any_folder == null).ToList();
					foreach (var folder in Search)
					{
						await Task.Run(() => getFoldersbyListAsync(folder.FolderName));
					}

					if (!App.ViewModel.Copy_ViewModel.Folders.Where(ee => ee.ScanSoubdirectories == false).ToList().Any())
					{
						break;
					}
				}
			});

			await Task.Run(async () =>
			{
				foreach (var item in App.ViewModel.Copy_ViewModel.Folders)
				{
					try
					{

						var files = await Task.Run(() =>
						{
							string[] _files = new string[0];
							try
							{
								_files = Directory.GetFiles(item.FolderName);
							}
							catch (Exception dd)
							{
								App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Errors.Add(dd.Message), DispatcherPriority.Normal);
							}

							return Task.FromResult(_files);
						});

						foreach (var file in files)
						{
							string File = file;
							FileInfo fileInfo = await Task.Run(() => new FileInfo(File));

							App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Files.Add(new FileList { FileInfo = fileInfo }), DispatcherPriority.Background);
						}
					}
					catch (Exception e) { App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Errors.Add(e.Message), DispatcherPriority.Normal); }
				}
			});
		}

		private static async Task getFoldersbyListAsync(string sDir)
		{
			await Task.Run(async () =>
			{
				bool soubfolders = false;

				try
				{
					var folders = await Task.Run(() =>
					{
						string[] _folders = new string[0];
						try
						{
							_folders = Directory.GetDirectories(sDir);
						}
						catch (Exception dd)
						{
							App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Errors.Add(dd.Message), DispatcherPriority.Normal);
						}

						return Task.FromResult(_folders);
					});

					foreach (string d in folders)
					{
						soubfolders = true;
						string FolderName = d;

						App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Folders.Add(new FolderScan
						{
							FolderName = FolderName,
							ScanSoubdirectories = false,
							FolderContain_any_folder = null
						}));
					}
				}
				catch (System.Exception e)
				{
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Errors.Add(e.Message), DispatcherPriority.Normal);
				}

				if (App.ViewModel.Copy_ViewModel.Folders.FirstOrDefault(ee => ee.FolderName == sDir) == null)
				{
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Folders.Add(new FolderScan
					{
						FolderName = sDir,
						ScanSoubdirectories = true,
						FolderContain_any_folder = soubfolders
					}));
				}
				else
				{
					foreach (var item in App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Folders.Where(ee => ee.FolderName == sDir)))
					{
						item.ScanSoubdirectories = true;
						item.FolderContain_any_folder = soubfolders;
						item.FolderName = sDir;
					}
				}
			});
		}


		public static Task CopyDirectory(string destinationDir, CancellationToken cancellationToken, MyDriversItemViewModel driver)
		{
			App.ViewModel.Copy_ViewModel.FileProcessed = 0;
			App._Dispatcher.Invoke(() => App.ViewModel.StatusViewModel.IsIndeterminate = false, DispatcherPriority.Background);

			try
			{
				Directory.CreateDirectory(destinationDir);
				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - hlavní složka vytvořena"), DispatcherPriority.Normal);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - hlavní složka vytvořena"), DispatcherPriority.Normal);
			}
			catch
			{
				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - nelze vytvořit hlavní skožku"), DispatcherPriority.Normal);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - nelze vytvořit hlavní skožku"), DispatcherPriority.Normal);
			}

			var files = App.ViewModel.Copy_ViewModel.Files;
			foreach (var file in files)
			{
				string targetFilePath = file.FileInfo.FullName.Replace(driver.Name, destinationDir);

				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - příprava souboru " + file.FileInfo.FullName + " do " + targetFilePath), DispatcherPriority.Normal);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - příprava souboru " + file.FileInfo.FullName + " do " + targetFilePath), DispatcherPriority.Normal);

				CopyFile(file, cancellationToken, destinationDir, targetFilePath);

				if (cancellationToken.IsCancellationRequested)
				{
					targetFilePath = "";
					App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - požadavek na přerušení kopírování"), DispatcherPriority.Normal);
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - požadavek na přerušení kopírování"), DispatcherPriority.Normal);

					cancellationToken.ThrowIfCancellationRequested();
				}
			}

			return Task.CompletedTask;
		}

		private static Task CopyFile(ViewModel.FileList file, CancellationToken cancellationToken, string destinationDir, string targetFilePath)
		{
			App.ViewModel.Copy_ViewModel.Repeats = 0;
			App.ViewModel.Copy_ViewModel.FileProcessed++;

			var CIL = new FileInfo(targetFilePath);

			Stopwatch dobakopirovaní = new Stopwatch();
			dobakopirovaní.Start();

			if (!Directory.Exists(targetFilePath))
			{
				try
				{
					Directory.CreateDirectory(CIL.DirectoryName);
					App._Dispatcher.Invoke(() => file.FolderCreated = true, DispatcherPriority.Background);
					App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - kopíruji soubor složka " + CIL.DirectoryName + " vytvořena"), DispatcherPriority.Normal);
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - kopíruji soubor složka " + CIL.DirectoryName + " vytvořena"), DispatcherPriority.Normal);

				}
				catch (Exception ex)
				{
					App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - nelze vytvořit hlavní skožku"), DispatcherPriority.Normal);
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - nelze vytvořit hlavní skožku"), DispatcherPriority.Normal);

					App._Dispatcher.Invoke(() => file.FolderCreated = false, DispatcherPriority.Background);
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Errors.Add(targetFilePath + ": \n" + ex.Message), DispatcherPriority.Background);
				}
			}

			var Task = MainCopyProcess(file, cancellationToken, destinationDir, targetFilePath);

			if (Task.IsCanceled)
			{
				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - počet pokusů překročen - přeskočení souboru"), DispatcherPriority.Normal);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - počet pokusů překročen - přeskočení souboru"), DispatcherPriority.Normal);
				return Task.CompletedTask;
			}

			if (cancellationToken.IsCancellationRequested)
			{
				targetFilePath = "";
				return Task.CompletedTask;
			}

			App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Procenta = ((double)App.ViewModel.Copy_ViewModel.FileProcessed / (double)App.ViewModel.Copy_ViewModel.Files.Count) * 100, DispatcherPriority.Background);
			App._Dispatcher.Invoke(() => file.FileCopy_dur = dobakopirovaní.Elapsed, DispatcherPriority.Background);

			return Task.CompletedTask;
		}


		private static Task MainCopyProcess(ViewModel.FileList file, CancellationToken cancellationToken, string destinationDir, string targetFilePath)
		{
		repeat:;
			App.ViewModel.Copy_ViewModel.Repeats++;
			App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - kopíruji soubor " + file.FileInfo.FullName + " do " + targetFilePath + " - opakování " + (App.ViewModel.Copy_ViewModel.Repeats - 1) + "x"), DispatcherPriority.Normal);

			App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - kopíruji soubor " + file.FileInfo.FullName + " do " + targetFilePath + " - opakování " + (App.ViewModel.Copy_ViewModel.Repeats - 1) + "x"), DispatcherPriority.Normal);

			if (App.ViewModel.Copy_ViewModel.Repeats > App.ViewModel.SelectedRepeat?.Počet)
			{
				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - počet pokusů překročen - požadavek o přeskočení souboru"), DispatcherPriority.Normal);

				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - počet pokusů překročen - požadavek o přeskočení souboru"), DispatcherPriority.Normal);

				return Task.FromException(new Exception("Překročen počet pokusů"));
			}

			bool overwrite = false;
			try
			{
				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("Zjištuji zda soubor existuje"), DispatcherPriority.Normal);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("Zjištuji zda soubor existuje"), DispatcherPriority.Normal);

				if (File.Exists(targetFilePath))
				{
					switch (App.ViewModel.Overwrite)
					{
						case true:
							overwrite = true;
							App._Dispatcher.Invoke(() => App.ViewModel.Title = ("Povoleno přepisování souborů"), DispatcherPriority.Normal);
							App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("Povoleno přepisování souborů"), DispatcherPriority.Normal);
							FileDelete(targetFilePath);
							break;

						case false:
							overwrite = false;
							App._Dispatcher.Invoke(() => App.ViewModel.Title = ("Zakázáno přepisování souborů"), DispatcherPriority.Normal);
							App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("Zakázáno přepisování souborů"), DispatcherPriority.Normal);
							return Task.CompletedTask;

						case null:
							{
								var FileExist = MessageBox.Show("Soubor " + targetFilePath + " již existuje\nChcete tento soubor přepsat, přeskočit, nebo ukončit kopírování?\n\n", "Existující soubor", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

								switch (FileExist)
								{
									case MessageBoxResult.Yes:
										overwrite = true;
										App._Dispatcher.Invoke(() => App.ViewModel.Title = ("Povoleno přepisování souborů"), DispatcherPriority.Normal);
										App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("Povoleno přepisování souborů"), DispatcherPriority.Normal);
										FileDelete(targetFilePath);
										break;

									case MessageBoxResult.No:
										App._Dispatcher.Invoke(() => App.ViewModel.Title = ("Zakázáno přepisování souborů"), DispatcherPriority.Normal);
										App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("Zakázáno přepisování souborů"), DispatcherPriority.Normal);
										overwrite = false;
										return Task.CompletedTask;

									case MessageBoxResult.Cancel:
										App.ViewModel.Copy_ViewModel.CancellationTokenSource.Cancel();
										break;
								}

								break;
							}
					}
				}

				if (cancellationToken.IsCancellationRequested)
				{
					targetFilePath = "";
					App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - požadavek na přerušení kopírování"), DispatcherPriority.Normal);
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - požadavek na přerušení kopírování"), DispatcherPriority.Normal);
					return Task.FromException(new Exception());
				}

				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("DVD COPY - kopíruji soubor " + file.FileInfo.FullName + " do " + targetFilePath), DispatcherPriority.Normal);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("DVD COPY - kopíruji soubor " + file.FileInfo.FullName + " do " + targetFilePath), DispatcherPriority.Normal);

				if (App.ViewModel.Overwrite == false) { 
				file.FileInfo.CopyTo(targetFilePath, overwrite);

				App._Dispatcher.Invoke(() => file.FileCopy = true, DispatcherPriority.Background);
				}
			}

			catch (Exception ex)
			{
				App._Dispatcher.Invoke(() => file.FileCopy = false, DispatcherPriority.Background);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Errors.Add(targetFilePath + ": \n" + ex.Message), DispatcherPriority.Background);

				FileDelete(targetFilePath);

				if (App.ViewModel.FileError == true)
				{
					App._Dispatcher.Invoke(() => App.ViewModel.Title = ("Chyba kopírování - opakovaní čtení " + targetFilePath), DispatcherPriority.Normal);
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("Chyba kopírování - opakovaní čtení " + targetFilePath), DispatcherPriority.Normal);
					goto repeat;
				}

				if (App.ViewModel.FileError == false)
				{
					App._Dispatcher.Invoke(() => App.ViewModel.Title = ("Chyba kopírování - vynechání čtení " + targetFilePath), DispatcherPriority.Normal);
					App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("Chyba kopírování - vynechání čtení " + targetFilePath), DispatcherPriority.Normal);
					return Task.CompletedTask;
				}

				if (App.ViewModel.FileError == null)
				{
					var copyError = MessageBox.Show("Opakovat kopírování?\nOverwrite: " + overwrite + "\n" + ex.Message, "Opakovat kopírování?", MessageBoxButton.YesNoCancel);

					switch (copyError)
					{
						case MessageBoxResult.Yes:
							goto repeat;
						case MessageBoxResult.No:
							return Task.CompletedTask;
							break;
						case MessageBoxResult.Cancel:
							App.ViewModel.Copy_ViewModel.CancellationTokenSource.Cancel();
							break;
					}
				}
			}

			return Task.CompletedTask;
		}

		private static Task FileDelete(string targetFilePath)
		{
			try
			{
				File.Delete(targetFilePath);
				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("Soubor existuje a bylo povoleno přepsání - soubor byl smazán"), DispatcherPriority.Normal);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("Soubor existuje a bylo povoleno přepsání - soubor byl smazán"), DispatcherPriority.Normal);
			}
			catch (Exception ee)
			{
				App._Dispatcher.Invoke(() => App.ViewModel.Title = ("Chyba pro mazání souboru " + ee.Message), DispatcherPriority.Normal);
				App._Dispatcher.Invoke(() => App.ViewModel.Copy_ViewModel.Models.Add("Chyba pro mazání souboru " + ee.Message), DispatcherPriority.Normal);
			}

			return Task.CompletedTask;
		}
	}
}
