using Library;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdminClient {
    class QuestionToAmountAnswerConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is null)
                return null;
            if (value is not Question condition)
                throw new ArgumentException($"Исходное значение должно иметь тип {nameof(condition)}");
            if (targetType != typeof(string))
                throw new InvalidCastException();
            Question question = (Question)value;
            return question.FreeAnswers.Count + question.MultipleAnswers.Count + question.SingleAnswers.Count;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
