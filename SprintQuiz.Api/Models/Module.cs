using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class Module:NiveauPedagogique
    {
       
        [Required]
        [ForeignKey(nameof(Sprint))]
        public Guid SprintId { get; set; }
        public virtual Sprint Sprint { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<Cours> Cours { get; set; } = new List<Cours>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public virtual ICollection<QAQuestion> QAQuestions { get; set; } = new List<QAQuestion>();
        public virtual ICollection<Exercice> Exercices { get; set; } = new List<Exercice>();
        public virtual ICollection<ProgressionUtilisateur> Progressions { get; set; } = new List<ProgressionUtilisateur>();
    }
}

