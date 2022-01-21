using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Library {
    public partial class Question {

        public int Id { get; set; }
        public string Text { get; set; }
        public int QuestionTypeId { get; set; }
        public int SurveyId { get; set; }
        public bool IsRequired { get; set; }
        #nullable enable
        public QuestionType? Type { get; set; }
        public Survey? Survey { get; set; }

        public ICollection<FreeAnswer> FreeAnswers { get; set; } = new HashSet<FreeAnswer>();
        public ICollection<MultipleAnswer> MultipleAnswers { get; set; } = new HashSet<MultipleAnswer>();
        public ICollection<SingleAnswer> SingleAnswers { get; set; } = new HashSet<SingleAnswer>();

        public QuestionDTO ToDTO() =>
            new QuestionDTO() {
                Id = Id,
                Text = Text,
                QuestionTypeId = QuestionTypeId,
                SurveyId = SurveyId,
                IsRequired = IsRequired,
                Type = Type?.ToDTO(),

                FreeAnswers = FreeAnswers.Select(answer => answer.ToDTO()).ToList(),
                MultipleAnswers = MultipleAnswers.Select(answer => answer.ToDTO()).ToList(),
                SingleAnswers = SingleAnswers.Select(answer => answer.ToDTO()).ToList()
            };

        static public Question FromDTO(QuestionDTO questionDTO) =>
            new Question() {
                Id = questionDTO.Id,
                Text = questionDTO.Text,
                QuestionTypeId = questionDTO.QuestionTypeId,
                SurveyId = questionDTO.SurveyId,
                Type = QuestionType.FromDTO(questionDTO.Type),
                IsRequired = questionDTO.IsRequired,

                FreeAnswers = questionDTO.FreeAnswers.Select(answer => FreeAnswer.FromDTO(answer)).ToHashSet(),
                MultipleAnswers = questionDTO.MultipleAnswers.Select(answer => MultipleAnswer.FromDTO(answer)).ToHashSet(),
                SingleAnswers = questionDTO.SingleAnswers.Select(answer => SingleAnswer.FromDTO(answer)).ToHashSet()
            };

    }
}
