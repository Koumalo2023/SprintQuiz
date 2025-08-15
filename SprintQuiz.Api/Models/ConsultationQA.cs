using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SprintQuiz.Api.Models
{
    public class ConsultationQA
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Utilisateur))]
        public Guid UtilisateurId { get; set; }

        [Required]
        [ForeignKey(nameof(QAQuestion))]
        public Guid QAQuestionId { get; set; }

        [Required]
        public DateTime DateConsultation { get; set; } = DateTime.UtcNow;

        public bool? MarqueeComprise { get; set; }

        // Navigation properties
        public virtual Utilisateur Utilisateur { get; set; } = null!;
        public virtual QAQuestion QAQuestion { get; set; } = null!;
    }
}

