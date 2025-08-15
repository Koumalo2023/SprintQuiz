using SprintQuiz.Api.Models;

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
}

