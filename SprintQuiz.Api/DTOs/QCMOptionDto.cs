using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.DTOs
{
    public class QCMOptionDto
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string Texte { get; set; } = string.Empty;
        public bool EstCorrecte { get; set; }
        public string? Explication { get; set; }
    }

    public class CreateQCMOptionDto
    {
        [Required]
        [MaxLength(500)]
        public string Texte { get; set; } = string.Empty;

        [Required]
        public bool EstCorrecte { get; set; }

        [MaxLength(1000)]
        public string? Explication { get; set; }
    }

    public class UpdateQCMOptionDto
    {
        [MaxLength(500)]
        public string? Texte { get; set; }

        public bool? EstCorrecte { get; set; }

        [MaxLength(1000)]
        public string? Explication { get; set; }
    }

    

}

