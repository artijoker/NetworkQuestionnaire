using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.QuestionViewModel {
    public class MultipleAnswerQuestionViewModel : QuestionViewModel {

        public string QuestionText { get; }
        public ObservableCollection<Answer> Answers { get; }

        public override bool IsRequired { get; }

        public MultipleAnswerQuestionViewModel(Question question) {
            QuestionText = question.Text;
            IsRequired = question.IsRequired;
            Answers = new ObservableCollection<Answer>(
                question.MultipleAnswers.Select(answer => new Answer(answer.Text, answer.Employees.Count == 1? true : false)));
        }
    }
}
