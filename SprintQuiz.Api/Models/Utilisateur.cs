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

        // --- Nouveaux champs : Objectifs personnels ---
        public int ObjectifHebdomadaireQuiz { get; set; } = 5;
        public int ObjectifHebdomadaireFlashcards { get; set; } = 10;
        public int ObjectifHebdomadaireExercices { get; set; } = 7;
        public TimeSpan ObjectifTempsRevision { get; set; } = TimeSpan.FromMinutes(300);

        // Navigation properties
        public virtual StatistiquesGlobales? StatistiquesGlobales { get; set; }
        public virtual ICollection<TentativeQuiz> TentativesQuiz { get; set; } = new List<TentativeQuiz>();
        public virtual ICollection<ConsultationQA> ConsultationsQA { get; set; } = new List<ConsultationQA>();
        public virtual ICollection<ProgressionUtilisateur> Progressions { get; set; } = new List<ProgressionUtilisateur>();
        public virtual ICollection<InscriptionFormation> InscriptionsFormations { get; set; } = new List<InscriptionFormation>();
        public virtual ICollection<ConsultationExercice> ConsultationsExercice { get; set; } = new List<ConsultationExercice>();
    }
}

