using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.DTOs
{
    public class ProfilDto
    {
        public Guid Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public RoleUtilisateur Role { get; set; }
        public DateTime DateInscription { get; set; }
        
        // Statistiques et progression
        public StatistiquesGlobalesDto? StatistiquesGlobales { get; set; }
        public float ProgressionGlobale { get; set; }
        public DernieresActivitesDto? DernieresActivites { get; set; }
    }

    public class UpdateProfilDto
    {
        public string? Nom { get; set; }
        public string? Email { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class UploadPhotoDto
    {
        public string PhotoBase64 { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }

    public class DernieresActivitesDto
    {
        public DateTime? DerniereTentativeQuiz { get; set; }
        public string? DernierQuizTitre { get; set; }
        public DateTime? DerniereConsultationQA { get; set; }
        public string? DerniereQuestionQA { get; set; }
        public DateTime? DerniereActiviteGlobale { get; set; }
    }
}

