using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace COPY_DVD.ViewModel
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void UpdateUI([CallerMemberName] string tt = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(tt));
		}
	}
}
