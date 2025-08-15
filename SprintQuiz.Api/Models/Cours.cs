using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class Cours
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titre { get; set; } = string.Empty;

        [MaxLength(10000)]
        public string? Description { get; set; }

        [Required]
        public int Ordre { get; set; }

        [Required]
        [ForeignKey(nameof(Module))]
        public Guid ModuleId { get; set; }

        // Navigation properties
        public virtual Module Module { get; set; } = null!;
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public virtual ICollection<QAQuestion> QAQuestions { get; set; } = new List<QAQuestion>();
        public virtual ICollection<Exercice> Exercices { get; set; } = new List<Exercice>();
        public virtual ICollection<ProgressionUtilisateur> Progressions { get; set; } = new List<ProgressionUtilisateur>();
    }
}

