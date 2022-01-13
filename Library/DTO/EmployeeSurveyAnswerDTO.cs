using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library {
    public class EmployeeSurveyAnswerDTO {
        public EmployeeDTO Employee { get; set; }
        public SurveyDTO Survey { get; set; }
        public List<SingleAnswerDTO> SingleAnswers { get; set; } = new List<SingleAnswerDTO>();
        public List<MultipleAnswerDTO> MultipleAnswers { get; set; } = new List<MultipleAnswerDTO>();
        public List<TextFreeAnswerDTO> FreeAnswers { get; set; } = new List<TextFreeAnswerDTO>();
    }
}
