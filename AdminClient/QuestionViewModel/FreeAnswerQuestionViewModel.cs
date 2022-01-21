using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.QuestionViewModel {
    public class FreeAnswerQuestionViewModel : QuestionViewModel {
        public string QuestionText { get;}
        public string AnswerText { get; }

        public override bool IsRequired { get; }

        public FreeAnswerQuestionViewModel(Question question) {
            QuestionText = question.Text;
            IsRequired = question.IsRequired;
            AnswerText = question.FreeAnswers.Single().EmployeeFreeAnswers.Single().Text;
        }
       
    }
}
