using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class EtapeResolution
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Exercice))]
        public Guid ExerciceId { get; set; }

        [Required]
        public int Ordre { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Navigation
        public virtual Exercice Exercice { get; set; } = null!;
    }
}
