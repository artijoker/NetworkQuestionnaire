using System;
using System.Collections.Generic;
using System.IO;


namespace Library {
    public partial class FreeAnswer {

        public int Id { get; set; }
        public int QuestionId { get; set; }

        public Question Question { get; set; }

        public ICollection<EmployeeFreeAnswer> EmployeeFreeAnswers { get; set; } = new HashSet<EmployeeFreeAnswer>();

        public FreeAnswerDTO ToDTO() => 
            new FreeAnswerDTO() { Id = Id, QuestionId = QuestionId };

        static public FreeAnswer FromDTO(FreeAnswerDTO answerDTO) => 
            new FreeAnswer() { Id = answerDTO.Id, QuestionId = answerDTO.QuestionId };
    }
}
