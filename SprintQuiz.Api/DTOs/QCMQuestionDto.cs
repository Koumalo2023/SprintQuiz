using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.DTOs
{
    public class QCMQuestionDto
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Intitule { get; set; } = string.Empty;
        public NiveauDifficulte NiveauDifficulte { get; set; }
        public string? Explication { get; set; }
        public List<QCMOptionDto> Options { get; set; } = new();
    }

    public class CreateQCMQuestionDto
    {
        public string Intitule { get; set; } = string.Empty;
        public NiveauDifficulte NiveauDifficulte { get; set; }
        public string? Explication { get; set; }
        public List<CreateQCMOptionDto> Options { get; set; } = new();
    }

    public class UpdateQCMQuestionDto
    {
        public string? Intitule { get; set; }
        public NiveauDifficulte? NiveauDifficulte { get; set; }
        public string? Explication { get; set; }
    }
}

