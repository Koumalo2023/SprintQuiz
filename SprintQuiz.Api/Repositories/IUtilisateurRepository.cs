using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public interface IUtilisateurRepository
    {
        Task<IEnumerable<Utilisateur>> GetAllAsync();
        Task<Utilisateur?> GetByIdAsync(Guid id);
        Task<Utilisateur?> GetByEmailAsync(string email);
        Task<Utilisateur> CreateAsync(Utilisateur utilisateur);
        Task<Utilisateur> UpdateAsync(Utilisateur utilisateur);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> EmailExistsAsync(string email);
        Task<Utilisateur?> GetByIdWithDetailsAsync(Guid id);
    }
}

