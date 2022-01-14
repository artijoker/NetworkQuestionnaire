using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClient.QuestionViewModel {

    public class SingleAnswerQuestionViewModel : QuestionViewModel {

        public string QuestionText { get; }
        public ObservableCollection<RadioButtonViewModel> Answers { get; }

        public override bool IsRequired { get; }

        public SingleAnswerQuestionViewModel(Question question) {
            QuestionText = question.Text;
            IsRequired = question.IsRequired;
            Answers = new ObservableCollection<RadioButtonViewModel>(
                question.SingleAnswers.Select(answer => new RadioButtonViewModel(answer, question.Id)));
        }
        public override void SaveAnswerEmployee(EmployeeSurveyAnswer userAnswer) {
            if (IsThereAnswer()) 
                userAnswer.SingleAnswersIds.Add(Answers.Single(answer => answer.IsChecked).Answer.Id);
        }

        public override bool IsThereAnswer() => Answers.Any(answer => answer.IsChecked == true);

    }
}
