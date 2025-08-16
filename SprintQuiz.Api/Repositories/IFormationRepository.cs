using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public interface IFormationRepository
    {
        Task<IEnumerable<Formation>> GetAllAsync();
        Task<Formation?> GetByIdAsync(Guid id);
        Task<Formation?> GetByIdWithSprintsAsync(Guid id);
        Task<Formation> CreateAsync(Formation formation);
        Task<Formation> UpdateAsync(Formation formation);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<Formation>> GetByOrdreAsync();
    }
}
