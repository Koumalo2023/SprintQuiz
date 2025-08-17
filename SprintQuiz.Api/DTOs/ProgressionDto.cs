using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.DTOs
{
    public class ProgressionUtilisateurDto
    {
        public Guid Id { get; set; }
        public Guid UtilisateurId { get; set; }
        public NiveauEnum Niveau { get; set; }
        public Guid NiveauId { get; set; }
        public string NiveauNom { get; set; } = string.Empty;
        public float PourcentageComplet { get; set; }
        public DateTime DerniereActivite { get; set; }
        public DateTime DateCreation { get; set; }
    }

    public class StatistiquesGlobalesDto
    {
        public Guid UtilisateurId { get; set; }
        public int TotalQuiz { get; set; }
        public float MoyenneScore { get; set; }
        public float TauxReussite { get; set; }
        public TimeSpan TempsTotalRevision { get; set; }
        public int QuestionsQRConsultees { get; set; }
        public int CoursTermines { get; set; }
        public int ModulesTermines { get; set; }
    }

    
}

