using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library {
    public class SingleAnswerDTO {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }
        public List<EmployeeDTO> Employees { get; set; } = new List<EmployeeDTO>();
    }
}
