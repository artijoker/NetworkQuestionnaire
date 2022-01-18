using Library;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdminClient {
    class QuestionTypeToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is null)
                return null;
            if (value is not QuestionType condition)
                throw new ArgumentException($"Исходное значение должно иметь тип {nameof(condition)}");
            if (targetType != typeof(string))
                throw new InvalidCastException();
            QuestionType type = (QuestionType)value;
            if (type.Type == "Single")
                return "Один из списка";
            else if (type.Type == "Multiple")
                return "Несколько из списка";
            else if (type.Type == "Free")
                return "Свободный ответ";
            else
                throw new InvalidCastException();
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
