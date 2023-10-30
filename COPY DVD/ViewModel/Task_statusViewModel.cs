using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace COPY_DVD.ViewModel
{
	public class Task_statusViewModel:ViewModelBase
	{
		private Cursor cursor;
		private Visibility copyStatus_visibility;
		private bool isIndeterminate;
		private string stopcontent;
		public Task_statusViewModel()
		{
			Cursor = Cursors.Wait;
			CopyStatus_visibility = Visibility.Hidden;
			IsIndeterminate = true;
			StopContent = "Přerušit";
		}

		public string StopContent
		{
			get => stopcontent;
			set
			{
				stopcontent = value;
				UpdateUI();
			}
		}

		public Cursor Cursor
		{
			get => cursor;

			set
			{
				cursor = value;
				UpdateUI();
			}
		}

		public Visibility CopyStatus_visibility
		{
			get => copyStatus_visibility;

			set
			{
				copyStatus_visibility = value;
				UpdateUI();
			}
		}
		public bool IsIndeterminate
		{
			get
			{
				return isIndeterminate;
			}
			set
			{
				isIndeterminate = value;
				UpdateUI();
			}
		}
	}
}
