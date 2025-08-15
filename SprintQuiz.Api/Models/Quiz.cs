using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class Quiz
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titre { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public NiveauEnum Niveau { get; set; }

        [Required]
        public Guid NiveauId { get; set; }

        [Required]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        [Timestamp]
        [ConcurrencyCheck]
        public byte[] Version { get; set; }

        // Navigation properties
        public virtual ICollection<QCMQuestion> Questions { get; set; } = new List<QCMQuestion>();
        public virtual ICollection<TentativeQuiz> Tentatives { get; set; } = new List<TentativeQuiz>();

        // Navigation properties conditionnelles selon le niveau
        public virtual Sprint? Sprint { get; set; }
        public virtual Module? Module { get; set; }
        public virtual Cours? Cours { get; set; }
    }
}

