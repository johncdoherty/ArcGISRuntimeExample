using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ArcGISRuntimeExample.Converters
{
    public sealed class VisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; } 

        public bool UseHidden { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolVal = System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);

            boolVal ^= IsReversed;

            if (boolVal)
            {
                return Visibility.Visible;
            }

            return this.UseHidden ? Visibility.Hidden : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolVal = ((value is Visibility) && (((Visibility)value) == Visibility.Visible));

            return boolVal ^ IsReversed;
        }
    }
}
