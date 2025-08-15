using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class QCMQuestion
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Quiz))]
        public Guid QuizId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Intitule { get; set; } = string.Empty;

        [Required]
        public NiveauDifficulte NiveauDifficulte { get; set; }

        [MaxLength(2000)]
        public string? Explication { get; set; }

        // Navigation properties
        public virtual Quiz Quiz { get; set; } = null!;
        public virtual ICollection<QCMOption> Options { get; set; } = new List<QCMOption>();
        public virtual ICollection<ReponseUtilisateurQCM> ReponsesUtilisateur { get; set; } = new List<ReponseUtilisateurQCM>();
    }
}

