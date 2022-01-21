using System.Collections.Generic;
using System.Linq;

namespace Library {
    public partial class SingleAnswer {

        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }

        public Question Question { get; set; }

        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();

        public SingleAnswerDTO ToDTO() =>
           new SingleAnswerDTO() { 
               Id = Id, 
               QuestionId = QuestionId, 
               Text = Text,
               Employees = Employees.Select(employee => employee.ToDTO()).ToList()
           };

        public static SingleAnswer FromDTO(SingleAnswerDTO answerDTO) =>
            new SingleAnswer() { 
                Id = answerDTO.Id, 
                QuestionId = answerDTO.QuestionId, 
                Text = answerDTO.Text,
                Employees = answerDTO.Employees.Select(employee => Employee.FromDTO(employee)).ToList()
            };

    }
}
