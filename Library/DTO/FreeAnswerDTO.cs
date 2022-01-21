using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library {
    public class FreeAnswerDTO {
        public int Id { get; set; }
        public int QuestionId { get; set; }

        public List<EmployeeFreeAnswerDTO> EmployeesFreeAnswers { get; set; } = new List<EmployeeFreeAnswerDTO>();
    }
}
