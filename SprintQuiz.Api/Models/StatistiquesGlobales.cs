using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class StatistiquesGlobales
    {
        [Key]
        [ForeignKey(nameof(Utilisateur))]
        public Guid UtilisateurId { get; set; }

        [Required]
        public int TotalQuiz { get; set; }

        [Required]
        public float MoyenneScore { get; set; }

        [Required]
        [Range(0.0, 1.0)]
        public float TauxReussite { get; set; }

        [Required]
        public TimeSpan TempsTotalRevision { get; set; }

        [Required]
        public int QuestionsQRConsultees { get; set; }

        [Required]
        public int CoursTermines { get; set; }

        [Required]
        public int ModulesTermines { get; set; }

        // Navigation property
        public virtual Utilisateur Utilisateur { get; set; } = null!;
    }
}

