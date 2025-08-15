using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class ConsultationExercice
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Utilisateur))]
        public Guid UtilisateurId { get; set; }

        [Required]
        [ForeignKey(nameof(Exercice))]
        public Guid ExerciceId { get; set; }

        [Required]
        public DateTime DateConsultation { get; set; } = DateTime.UtcNow;

        public bool? MarqueeCompris { get; set; } // L'utilisateur indique s'il a compris
        public bool AConsulteSolution { get; set; } = false;
        public bool AUtiliseIndices { get; set; } = false;
        public int TentativesAvantSolution { get; set; } = 0;

        // Navigation properties
        public virtual Utilisateur Utilisateur { get; set; } = null!;
        public virtual Exercice Exercice { get; set; } = null!;
    }
}
