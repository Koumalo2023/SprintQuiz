using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class QCMOption
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Texte { get; set; } = string.Empty;

        [Required]
        public bool EstCorrecte { get; set; }
        [MaxLength(1000)]
        public string? Explication { get; set; }

        // Navigation properties
        public virtual QCMQuestion Question { get; set; } = null!;
        public virtual ICollection<ReponseUtilisateurQCM> ReponsesUtilisateur { get; set; } = new List<ReponseUtilisateurQCM>();
    }
}

