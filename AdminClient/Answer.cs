using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient {
    public class Answer {

        public int Id { get; }
        public string Text { get; }
        public Answer(int id, string text) {
            Id = id;
            Text = text;
        }
    }
}
