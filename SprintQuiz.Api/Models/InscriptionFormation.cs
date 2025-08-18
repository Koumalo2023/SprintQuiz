using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class InscriptionFormation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UtilisateurId { get; set; }

        [Required]
        public Guid FormationId { get; set; }

        public StatutInscription Statut { get; set; } = StatutInscription.Actif;

        [Required]
        public DateTime DateInscription { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Utilisateur Utilisateur { get; set; } = null!;
        public virtual Formation Formation { get; set; } = null!;
    }
}
