using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    public class Sprint:NiveauPedagogique
    {
        public Guid FormationId { get; set; }
        public virtual Formation Formation { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public virtual ICollection<QAQuestion> QAQuestions { get; set; } = new List<QAQuestion>();
        public virtual ICollection<Exercice> Exercices { get; set; } = new List<Exercice>();
        public virtual ICollection<ProgressionUtilisateur> Progressions { get; set; } = new List<ProgressionUtilisateur>();
    }
}

