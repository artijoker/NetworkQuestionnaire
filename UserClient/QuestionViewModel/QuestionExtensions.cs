using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UserClient.QuestionViewModel;

namespace UserClient {

    static public class QuestionExtensions {

        static public QuestionViewModel.QuestionViewModel GetViewModel(this Question question) {

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
