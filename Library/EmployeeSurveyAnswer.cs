using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library {
    public class EmployeeSurveyAnswer {
        public int EmployeeId { get; set; }
        public int SurveyId { get; set; }
        public List<int> SingleAnswersIds { get; set; } = new();
        public List<int> MultipleAnswersIds { get; set; } = new();
        public List<(int, string)> FreeAnswersIds { get; set; } = new();
    }
}
