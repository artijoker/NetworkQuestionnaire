using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClient.QuestionViewModel {

    public abstract class QuestionViewModel : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void SaveAnswerEmployee(EmployeeSurveyAnswer employeeAnswer);
        public abstract bool IsThereAnswer();
        public abstract bool IsRequired { get; }
        public void OnPropertyChanged(string property) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        static public QuestionViewModel GetViewModel(Question question) {

            if (question.Type.Type == "Single") {
                return new SingleAnswerQuestionViewModel(question);
            }
            else if (question.Type.Type == "Multiple") {
                return new MultipleAnswerQuestionViewModel(question);
            }
            else if (question.Type.Type == "Free") {
                return new FreeAnswerQuestionViewModel(question);
            }
            else
                throw new FormatException();
        }
    }
}
