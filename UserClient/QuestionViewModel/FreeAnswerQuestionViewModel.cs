using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClient.QuestionViewModel {
    public class FreeAnswerQuestionViewModel : QuestionViewModel {

        private string _answerText;

        public FreeAnswer Answer { get; }
        public string QuestionText { get; set; }
        public string AnswerText {
            get => _answerText;
            set {
                _answerText = value;
                if (_answerText == value) return;
                OnPropertyChanged(nameof(AnswerText));
            }
        }

        public override bool IsRequired { get; }

        public FreeAnswerQuestionViewModel(Question question) {
            QuestionText = question.Text;
            IsRequired = question.IsRequired;
            Answer = question.FreeAnswers.Single();
        }
        public override void SaveAnswerEmployee(EmployeeSurveyAnswer employeeAnswer) {
            var answer = (Answer.Id, AnswerText);
            if (IsThereAnswer())
                employeeAnswer.FreeAnswersIds.Add(answer);
        }

        public override bool IsThereAnswer() => string.IsNullOrEmpty(AnswerText) ? false : true;
    }
}
