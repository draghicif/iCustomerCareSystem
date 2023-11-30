using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Data;
using iCustomerCareSystem.Models;

namespace iCustomerCareSystem.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (Enum.TryParse(parameter.ToString(), out StatusId enumValue))
            {
                return enumValue.Equals(value);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                if (Enum.TryParse(parameter.ToString(), out StatusId enumValue))
                {
                    return enumValue;
                }
            }

            return Binding.DoNothing;
        }
    }
}
