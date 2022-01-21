using System.Collections.Generic;
using System.Linq;


namespace Library {
    public partial class FreeAnswer {

        public int Id { get; set; }
        public int QuestionId { get; set; }

        public Question Question { get; set; }

        public ICollection<EmployeeFreeAnswer> EmployeeFreeAnswers { get; set; } = new HashSet<EmployeeFreeAnswer>();

        public FreeAnswerDTO ToDTO() =>
            new FreeAnswerDTO() {
                Id = Id,
                QuestionId = QuestionId,
                EmployeesFreeAnswers = EmployeeFreeAnswers.Select(e => e.ToDTO())
                .ToList()
            };

        static public FreeAnswer FromDTO(FreeAnswerDTO answerDTO) =>
            new FreeAnswer() {
                Id = answerDTO.Id,
                QuestionId = answerDTO.QuestionId,
                EmployeeFreeAnswers = answerDTO.EmployeesFreeAnswers.Select(e => EmployeeFreeAnswer.FromDTO(e))
                .ToList()
            };
    }
}
