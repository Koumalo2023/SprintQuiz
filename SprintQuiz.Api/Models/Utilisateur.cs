using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class Utilisateur
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string MotDePasse { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? PhotoUrl { get; set; }

        [Required]
        public RoleUtilisateur Role { get; set; }

        [Required]
        public DateTime DateInscription { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<TentativeQuiz> TentativesQuiz { get; set; } = new List<TentativeQuiz>();
        public virtual ICollection<ConsultationQA> ConsultationsQA { get; set; } = new List<ConsultationQA>();
        public virtual ICollection<ProgressionUtilisateur> Progressions { get; set; } = new List<ProgressionUtilisateur>();
        
        // Ajout manquant : Consultations d'exercices
        public virtual ICollection<ConsultationExercice> ConsultationsExercice { get; set; } = new List<ConsultationExercice>();
        public virtual StatistiquesGlobales? StatistiquesGlobales { get; set; }
    }
}

