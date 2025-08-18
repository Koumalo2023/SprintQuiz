using SprintQuiz.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace SprintQuiz.Api.DTOs
{
    public class UtilisateurDto
    {
        public Guid Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public RoleUtilisateur Role { get; set; }
        public DateTime DateInscription { get; set; }

        // --- Objectifs ---
        public int ObjectifHebdomadaireQuiz { get; set; }
        public int ObjectifHebdomadaireFlashcards { get; set; }
        public int ObjectifHebdomadaireExercices { get; set; }
        public TimeSpan ObjectifTempsRevision { get; set; }
    }

    public class CreateUtilisateurDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
        public RoleUtilisateur Role { get; set; }
    }

    public class UpdateUtilisateurDto
    {
        public string? Nom { get; set; }
        public string? Email { get; set; }
        public string? PhotoUrl { get; set; }
        public RoleUtilisateur? Role { get; set; }

        // --- Mise à jour des objectifs ---
        public int? ObjectifHebdomadaireQuiz { get; set; }
        public int? ObjectifHebdomadaireFlashcards { get; set; }
        public int? ObjectifHebdomadaireExercices { get; set; }
        public TimeSpan? ObjectifTempsRevision { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UtilisateurDto Utilisateur { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // DTOs/InscriptionDto.cs
    public class InscriptionDto
    {
        public Guid Id { get; set; }
        public Guid UtilisateurId { get; set; }
        public string NomUtilisateur { get; set; } = string.Empty;
        public Guid FormationId { get; set; }
        public string NomFormation { get; set; } = string.Empty;
        public DateTime DateInscription { get; set; }
        public StatutInscription Statut { get; set; } = StatutInscription.Actif;
    }

    // DTOs/CreateInscriptionDto.cs
    public class CreateInscriptionDto
    {
        [Required]
        public Guid UtilisateurId { get; set; }

        [Required]
        public Guid FormationId { get; set; }
    }



}

