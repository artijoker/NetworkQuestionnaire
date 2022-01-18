using System;
using System.Collections.Generic;

namespace Library {
    public partial class QuestionType {
        
        public int Id { get; set; }
        public string Type { get; set; }

        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();

        public QuestionTypeDTO ToDTO() => new QuestionTypeDTO() { Id = Id, Type = Type };
        #nullable enable
        static public QuestionType? FromDTO(QuestionTypeDTO? typeDTO) {
            if (typeDTO is null) return null;
            return new QuestionType() { Id = typeDTO.Id, Type = typeDTO.Type };
        }
    }
}
