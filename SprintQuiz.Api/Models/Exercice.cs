using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class Exercice
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Enonce { get; set; } = string.Empty;

        [Required]
        [MaxLength(5000)]
        public string Solution { get; set; } = string.Empty;

        [Required]
        public NiveauEnum Niveau { get; set; }

        [Required]
        public Guid NiveauId { get; set; }
        public DateTime? DerniereModification { get; set; }

        [Required]
        public NiveauDifficulte NiveauDifficulte { get; set; }
        public int DureeEstimee { get; set; } = 0;

        public TypeExercice Type { get; set; } = TypeExercice.Basique;

        public List<string>? Tags { get; set; }

        [Required]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        // Ajout des collections de navigation
        public virtual ICollection<Indice> Indices { get; set; } = new List<Indice>();
        public virtual ICollection<EtapeResolution> EtapesResolution { get; set; } = new List<EtapeResolution>();
        public virtual ICollection<ConsultationExercice> Consultations { get; set; } = new List<ConsultationExercice>();

    }
}
