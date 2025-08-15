using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class Module
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nom { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string? Description { get; set; }

        [Required]
        public int Ordre { get; set; }

        [Required]
        [ForeignKey(nameof(Sprint))]
        public Guid SprintId { get; set; }

        // Navigation properties
        public virtual Sprint Sprint { get; set; } = null!;
        public virtual ICollection<Cours> Cours { get; set; } = new List<Cours>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public virtual ICollection<QAQuestion> QAQuestions { get; set; } = new List<QAQuestion>();
        public virtual ICollection<Exercice> Exercices { get; set; } = new List<Exercice>();
        public virtual ICollection<ProgressionUtilisateur> Progressions { get; set; } = new List<ProgressionUtilisateur>();
    }
}

