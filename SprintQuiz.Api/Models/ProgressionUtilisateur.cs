using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class ProgressionUtilisateur
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Utilisateur))]
        public Guid UtilisateurId { get; set; }

        [Required]
        public NiveauEnum Niveau { get; set; }

        [Required]
        public Guid NiveauId { get; set; }

        [Required]
        [Range(0.0, 1.0)]
        public float PourcentageComplet { get; set; }

        [Required]
        public DateTime DerniereActivite { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Utilisateur Utilisateur { get; set; } = null!;

        // Navigation properties conditionnelles selon le niveau
        public virtual Sprint? Sprint { get; set; }
        public virtual Module? Module { get; set; }
        public virtual Cours? Cours { get; set; }
    }
}

