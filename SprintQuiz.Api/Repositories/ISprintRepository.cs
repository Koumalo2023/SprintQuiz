using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public interface ISprintRepository
    {
        Task<IEnumerable<Sprint>> GetAllAsync();
        Task<Sprint?> GetByIdAsync(Guid id);
        Task<Sprint?> GetByIdWithModulesAsync(Guid id);
        Task<Sprint> CreateAsync(Sprint sprint);
        Task<Sprint> UpdateAsync(Sprint sprint);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<Sprint>> GetByOrdreAsync();
    }
}

