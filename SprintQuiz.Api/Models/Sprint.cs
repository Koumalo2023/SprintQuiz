using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class Sprint
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nom { get; set; } = string.Empty;

        [MaxLength(3000)]
        public string? Description { get; set; }

        [Required]
        public int Ordre { get; set; }

        // Navigation properties
        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public virtual ICollection<QAQuestion> QAQuestions { get; set; } = new List<QAQuestion>();
        public virtual ICollection<Exercice> Exercices { get; set; } = new List<Exercice>();
        public virtual ICollection<ProgressionUtilisateur> Progressions { get; set; } = new List<ProgressionUtilisateur>();
    }
}

