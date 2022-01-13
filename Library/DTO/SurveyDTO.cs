using System;
using System.Collections.Generic;

namespace Library {
    public class SurveyDTO {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<QuestionDTO> Questions { get; set; } = new List<QuestionDTO>();
    }
}
