using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class QAQuestion
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Question { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Reponse { get; set; } = string.Empty;

        [Required]
        public NiveauEnum Niveau { get; set; }

        [Required]
        public Guid NiveauId { get; set; }
        public int DureeEstimee { get; set; } = 0;

        [Required]
        public NiveauDifficulte NiveauDifficulte { get; set; }

        public List<string>? Tags { get; set; }

        [Required]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        public DateTime? DerniereModification { get; set; }

        // Navigation properties
        public virtual ICollection<ConsultationQA> Consultations { get; set; } = new List<ConsultationQA>();

        // Navigation properties conditionnelles selon le niveau
        public virtual Sprint? Sprint { get; set; }
        public virtual Module? Module { get; set; }
        public virtual Cours? Cours { get; set; }
    }
}

