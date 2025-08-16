namespace SprintQuiz.Api.Models
{
    public class Formation:NiveauPedagogique
    {
        // --- Relations ---
        public virtual ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
        public virtual ICollection<ProgressionUtilisateur> Progressions { get; set; } = new List<ProgressionUtilisateur>();
    }
}
