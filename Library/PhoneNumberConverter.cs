using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Library {
    public class PhoneNumberConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is null)
                return null;
            if (!(value is string condition))
                throw new ArgumentException($"Исходное значение должно иметь тип {nameof(condition)}");
            if (targetType != typeof(string))
                throw new InvalidCastException();

            return string.Format("{0:+# (###) ###-##-##}", long.Parse((string)value));
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
