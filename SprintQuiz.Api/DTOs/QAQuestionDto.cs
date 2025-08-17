using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.DTOs
{
    public class QAQuestionDto
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Reponse { get; set; } = string.Empty;
        public NiveauEnum Niveau { get; set; }
        public Guid NiveauId { get; set; }
        public NiveauDifficulte NiveauDifficulte { get; set; }
        public List<string>? Tags { get; set; }
        public DateTime DateCreation { get; set; } 
        public DateTime? DerniereModification { get; set; }

        public int DureeEstimee { get; set; }

        public DateTime? DerniereActivite { get; set; }
    }

    public class CreateQAQuestionDto
    {
        public string Question { get; set; } = string.Empty;
        public string Reponse { get; set; } = string.Empty;
        public NiveauEnum Niveau { get; set; }
        public Guid NiveauId { get; set; }
        public NiveauDifficulte NiveauDifficulte { get; set; }
        public List<string>? Tags { get; set; }
        public List<CreateQCMOptionDto> Options { get; set; } = new();
    }

    public class UpdateQAQuestionDto
    {
        public string? Question { get; set; }
        public string? Reponse { get; set; }
        public NiveauEnum? Niveau { get; set; }
        public Guid? NiveauId { get; set; }
        public NiveauDifficulte? NiveauDifficulte { get; set; }
        public List<string>? Tags { get; set; }
    }

    public class ConsultationQADto
    {
        public Guid Id { get; set; }
        public Guid UtilisateurId { get; set; }
        public Guid QAQuestionId { get; set; }
        public DateTime DateConsultation { get; set; }
        public bool? MarqueeComprise { get; set; }
    }

    public class CreateConsultationQADto
    {
        public Guid QAQuestionId { get; set; }
        public bool? MarqueeComprise { get; set; }
    }
}

