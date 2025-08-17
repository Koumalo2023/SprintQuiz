using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.DTOs
{
    public class ExerciceDto
    {
        public Guid Id { get; set; }
        public string Enonce { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string? SolutionResume { get; set; } // Résumé rapide
        public NiveauEnum Niveau { get; set; }
        public Guid NiveauId { get; set; }
        public NiveauDifficulte NiveauDifficulte { get; set; }
        public int DureeEstimee { get; set; }
        public DateTime? DerniereActivite { get; set; }
        public DateTime? DerniereModification { get; set; }
        public TypeExercice Type { get; set; }
        public List<string>? Tags { get; set; }
        public DateTime DateCreation { get; set; }

        public List<IndiceDto>? Indices { get; set; }
        public List<EtapeResolutionDto>? EtapesResolution { get; set; }
    }

    public class CreateExerciceDto
    {
        public string Enonce { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public string? SolutionResume { get; set; }
        public NiveauEnum Niveau { get; set; }
        public Guid NiveauId { get; set; }
        public NiveauDifficulte NiveauDifficulte { get; set; }
        public TypeExercice Type { get; set; }
        public List<string>? Tags { get; set; }
        public List<CreateIndiceDto>? Indices { get; set; }
        public List<CreateEtapeResolutionDto>? EtapesResolution { get; set; }
    }

    public class UpdateExerciceDto
    {
        public string? Enonce { get; set; }
        public string? Solution { get; set; }
        public NiveauEnum? Niveau { get; set; }
        public Guid? NiveauId { get; set; }
        public TypeExercice Type { get; set; }
        public NiveauDifficulte? NiveauDifficulte { get; set; }
        public List<string>? Tags { get; set; }
    }

    public class ConsultationExerciceDto
    {
        public Guid Id { get; set; }
        public Guid UtilisateurId { get; set; }
        public Guid ExerciceId { get; set; }
        public DateTime DateConsultation { get; set; }
        public bool? MarqueeCompris { get; set; }
    }
    public class CreateConsultationExerciceDto
    {
        public Guid ExerciceId { get; set; }
        public bool? MarqueeCompris { get; set; }
    }

    public class IndiceDto
    {
        public Guid Id { get; set; }
        public int Ordre { get; set; }
        public string Texte { get; set; } = string.Empty;
    }
    public class EtapeResolutionDto
    {
        public Guid Id { get; set; }
        public int Ordre { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class CreateIndiceDto
    {
        public int Ordre { get; set; } = 1;
        public string Texte { get; set; } = string.Empty;
    }

    public class UpdateIndiceDto
    {
        public int? Ordre { get; set; }
        public string? Texte { get; set; }
    }

    public class CreateEtapeResolutionDto
    {
        public int Ordre { get; set; } = 1;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateEtapeResolutionDto
    {
        public int? Ordre { get; set; }
        public string? Description { get; set; }
    }
}
