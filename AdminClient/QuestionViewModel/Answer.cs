using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient.QuestionViewModel {
    public class Answer {
        public string Text { get; }

        public bool IsChecked { get; }

        public Answer(string text, bool isChecked) {
            Text = text;
            IsChecked = isChecked;
        }
    }
}
