using System.Collections.Generic;
using System.Linq;

namespace Library {
    public partial class MultipleAnswer {

        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }

        public Question Question { get; set; }

        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();

        public MultipleAnswerDTO ToDTO() =>
            new MultipleAnswerDTO() { 
                Id = Id, 
                QuestionId = QuestionId, 
                Text = Text,
                Employees = Employees.Select(employee => employee.ToDTO()).ToList()
            };

        public static MultipleAnswer FromDTO(MultipleAnswerDTO answerDTO) =>
            new MultipleAnswer() { 
                Id = answerDTO.Id, 
                QuestionId = answerDTO.QuestionId, 
                Text = answerDTO.Text,
                Employees = answerDTO.Employees.Select(employee => Employee.FromDTO(employee)).ToList()
            };
    }
}
