using SprintQuiz.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.DTOs
{
    public class SprintDto
    {
        public Guid Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Ordre { get; set; }

        public bool EstActif { get; set; }
        public DateTime? DateOuverture { get; set; }
        public string? Objectifs { get; set; }
        public string? Resume { get; set; }
        public string? NotionsCles { get; set; }
        public int DureeEstimee { get; set; }
        public NiveauDifficulte DifficulteMoyenne { get; set; }
        public List<string> Tags { get; set; } = new();
        public int NombreQuiz { get; set; }
        public int NombreFlashcards { get; set; }
        public int NombreExercices { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DerniereModification { get; set; }
        public DateTime? DerniereActivite { get; set; } // Calculé dynamiquement
        public Guid FormationId { get; set; }
        public string? FormationNom { get; set; }
        public List<ModuleDto>? Modules { get; set; }
    }


    public class CreateSprintDto
    {
        [Required] 
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required] 
        public int Ordre { get; set; }

        public bool EstActif { get; set; } = true;
        public DateTime? DateOuverture { get; set; }
        public string? Objectifs { get; set; }
        public string? Resume { get; set; }
        public string? NotionsCles { get; set; }
        public Guid FormationId { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    public class UpdateSprintDto
    {
        public string? Nom { get; set; }
        public string? Description { get; set; }
        public int? Ordre { get; set; }

        public bool? EstActif { get; set; }
        public DateTime? DateOuverture { get; set; }
        public string? Objectifs { get; set; }
        public string? Resume { get; set; }
        public string? NotionsCles { get; set; }
        public Guid FormationId { get; set; }
        public List<string>? Tags { get; set; }
    }
}

