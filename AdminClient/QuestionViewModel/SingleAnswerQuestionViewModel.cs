using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.QuestionViewModel {

    public class SingleAnswerQuestionViewModel : QuestionViewModel {

        public string QuestionText { get; }
        public ObservableCollection<Answer> Answers { get; }

        public override bool IsRequired { get; }

        public SingleAnswerQuestionViewModel(Question question) {
            QuestionText = question.Text;
            IsRequired = question.IsRequired;
            Answers = new ObservableCollection<Answer>(
                question.SingleAnswers.Select(answer => new Answer(answer.Text, answer.Employees.Count == 1 ? true : false)));
        }

    }
}
