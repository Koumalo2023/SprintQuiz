using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public interface ICoursRepository
    {
        Task<IEnumerable<Cours>> GetAllAsync();
        Task<Cours?> GetByIdAsync(Guid id);
        Task<IEnumerable<Cours>> GetByModuleIdAsync(Guid moduleId);
        Task<Cours> CreateAsync(Cours cours);
        Task<Cours> UpdateAsync(Cours cours);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}

