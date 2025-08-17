using SprintQuiz.Api.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class TentativeQuiz
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Utilisateur))]
        public Guid UtilisateurId { get; set; }

        [Required]
        [ForeignKey(nameof(Quiz))]
        public Guid QuizId { get; set; }

        [Required]
        public float Score { get; set; }

        [Required]
        public bool Reussi { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public TimeSpan TempsPasse { get; set; }

        // Navigation properties
        public virtual Utilisateur Utilisateur { get; set; } = null!;
        public virtual Quiz Quiz { get; set; } = null!; 
        public virtual ICollection<ReponseUtilisateurQCM> ReponsesUtilisateur { get; set; } = new List<ReponseUtilisateurQCM>();
    }
}

