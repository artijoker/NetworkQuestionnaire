using System;
using System.Collections.Generic;
using System.Linq;

namespace Library {
    public partial class Survey {

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();

        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();

        public SurveyDTO ToDTO() =>
            new SurveyDTO() {
                Id = Id,
                Name = Name,

                Questions = Questions.Select(question => question.ToDTO()).ToList()
            };

        static public Survey FromDTO(SurveyDTO surveyDTO) =>
            new Survey() {
                Id = surveyDTO.Id,
                Name = surveyDTO.Name,

                Questions = surveyDTO.Questions.Select(question => Question.FromDTO(question)).ToHashSet()
            };

    }
}
