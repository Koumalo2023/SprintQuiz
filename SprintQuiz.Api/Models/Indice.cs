using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class Indice
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Exercice))]
        public Guid ExerciceId { get; set; }

        [Required]
        public int Ordre { get; set; } // 1er indice, 2e, etc.

        [Required]
        [MaxLength(500)]
        public string Texte { get; set; } = string.Empty;

        // Navigation
        public virtual Exercice Exercice { get; set; } = null!;
    }
}
