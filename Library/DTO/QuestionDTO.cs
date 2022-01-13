using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library {

    public class QuestionDTO {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionTypeId { get; set; }
        public int SurveyId { get; set; }
        public bool IsRequired { get; set; }

        public QuestionTypeDTO Type { get; set; }

        public ICollection<FreeAnswerDTO> FreeAnswers { get; set; } = new List<FreeAnswerDTO>();
        public ICollection<MultipleAnswerDTO> MultipleAnswers { get; set; } = new List<MultipleAnswerDTO>();
        public ICollection<SingleAnswerDTO> SingleAnswers { get; set; } = new List<SingleAnswerDTO>();
    }
}
