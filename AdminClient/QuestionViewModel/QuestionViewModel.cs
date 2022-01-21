using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.QuestionViewModel {

    public abstract class QuestionViewModel {
        public abstract bool IsRequired { get; }

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
