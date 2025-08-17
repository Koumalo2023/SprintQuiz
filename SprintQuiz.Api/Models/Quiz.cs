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
        public DateTime? DerniereModification { get; set; }

        public List<string>? Tags { get; set; }

        [Required]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public int DureeEstimee { get; set; } = 0;
        // --- Nouveaux champs ---
        public TypeQuiz Type { get; set; } = TypeQuiz.Entrainement;
        public bool MelangerQuestions { get; set; } = false;

        public DateTime Version { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<QCMQuestion> Questions { get; set; } = new List<QCMQuestion>();
        public virtual ICollection<TentativeQuiz> Tentatives { get; set; } = new List<TentativeQuiz>();
    }
}

