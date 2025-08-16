using SprintQuiz.Api.DTOs;

namespace SprintQuiz.Api.Services
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleDto>> GetAllAsync();
        Task<ModuleDto?> GetByIdAsync(Guid id, Guid? utilisateurId = null);
        Task<ModuleDto?> GetWithCoursAsync(Guid id, Guid? utilisateurId = null);
        Task<ModuleDto> CreateAsync(CreateModuleDto createDto);
        Task<ModuleDto?> UpdateAsync(Guid id, UpdateModuleDto updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<ModuleDto>> GetBySprintIdAsync(Guid sprintId, Guid? utilisateurId = null);
    }
}

