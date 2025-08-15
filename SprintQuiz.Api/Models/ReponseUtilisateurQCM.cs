using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class ReponseUtilisateurQCM
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Tentative))]
        public Guid TentativeId { get; set; }

        [Required]
        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }

        [Required]
        [ForeignKey(nameof(Option))]
        public Guid OptionId { get; set; }

        [Required]
        public bool EstCorrecte { get; set; }

        // Navigation properties
        public virtual TentativeQuiz Tentative { get; set; } = null!;
        public virtual QCMQuestion Question { get; set; } = null!;
        public virtual QCMOption Option { get; set; } = null!;
    }
}

