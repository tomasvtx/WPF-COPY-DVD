using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace COPY_DVD.Converter
{
	public class RadioState_IsDisabled : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
		bool? Val = value as bool?;

			if (Val == null)
				return false;

			if (Val == true)
				return false;

			if (Val == false)
				return true;

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool? Val = value as bool?;

			if (Val == null)
				return false;

			if (Val == true)
				return false;

			if (Val == false)
				return true;

			return false;
		}
	}
}
