using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public interface IModuleRepository
    {
        Task<IEnumerable<Module>> GetAllAsync();
        Task<Module?> GetByIdAsync(Guid id);
        Task<Module?> GetByIdWithCoursAsync(Guid id);
        Task<IEnumerable<Module>> GetBySprintIdAsync(Guid sprintId);
        Task<Module> CreateAsync(Module module);
        Task<Module> UpdateAsync(Module module);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}

