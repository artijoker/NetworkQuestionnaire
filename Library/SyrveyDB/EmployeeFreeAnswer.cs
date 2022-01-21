using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library {
    public class EmployeeFreeAnswer {
        public int FreeAnswerId { get; set; }
        public int EmployeeId { get; set; }
        public string Text { get; set; }

        public FreeAnswer FreeAnswer { get; set; }
        public Employee Employee { get; set; }

        public EmployeeFreeAnswerDTO ToDTO() =>
           new EmployeeFreeAnswerDTO() { 
               EmployeeId = EmployeeId,
               FreeAnswerId = FreeAnswerId,
               Text = Text
           };

        static public EmployeeFreeAnswer FromDTO(EmployeeFreeAnswerDTO employeeFreeAnswerDTO) =>
            new EmployeeFreeAnswer() {
                EmployeeId = employeeFreeAnswerDTO.EmployeeId,
                FreeAnswerId = employeeFreeAnswerDTO.FreeAnswerId,
                Text = employeeFreeAnswerDTO.Text
            };
    }
}
