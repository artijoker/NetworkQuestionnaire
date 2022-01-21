using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClient.QuestionViewModel {
    public class MultipleAnswerQuestionViewModel : QuestionViewModel {

        public string QuestionText { get; set; }
        public ObservableCollection<CheckBoxViewModel> Answers { get; }

        public override bool IsRequired { get; }

        public MultipleAnswerQuestionViewModel(Question question) {
            QuestionText = question.Text;
            IsRequired = question.IsRequired;
            Answers = new ObservableCollection<CheckBoxViewModel>(
                question.MultipleAnswers.Select(answer => new CheckBoxViewModel(answer)));
        }
        public override void SaveAnswerEmployee(EmployeeSurveyAnswer employeeAnswer) {
            if (IsThereAnswer())
                employeeAnswer.MultipleAnswersIds.AddRange(Answers.Where(answer => answer.IsChecked)
               .Select(answer => answer.Answer.Id));
        }

        public override bool IsThereAnswer() => Answers.Any(answer => answer.IsChecked == true);

    }
}
