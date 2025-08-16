using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.Models
{
    // Models/NiveauPedagogique.cs
    public abstract class NiveauPedagogique
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nom { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int Ordre { get; set; }

        // --- Fonctionnalités ajoutées ---
        public bool EstActif { get; set; } = true;

        public DateTime? DateOuverture { get; set; }
        [MaxLength(5000)]
        public string? Objectifs { get; set; }
        [MaxLength(10000)]
        public string? Resume { get; set; }
        [MaxLength(10000)]
        public string? NotionsCles { get; set; } // Glossaire ou mots-clés

        public int DureeEstimee { get; set; } = 0; // en minutes

        public NiveauDifficulte DifficulteMoyenne { get; set; } = NiveauDifficulte.Moyen;

        public List<string> Tags { get; set; } = new();

        // Compteurs (calculés ou mis à jour)
        public int NombreQuiz { get; set; } = 0;
        public int NombreFlashcards { get; set; } = 0;
        public int NombreExercices { get; set; } = 0;

        // Métadonnées
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public DateTime? DerniereModification { get; set; }
    }
}
