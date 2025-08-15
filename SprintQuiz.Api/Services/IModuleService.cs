using SprintQuiz.Api.DTOs;

namespace SprintQuiz.Api.Services
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleDto>> GetAllModulesAsync();
        Task<ModuleDto?> GetModuleByIdAsync(Guid id);
        Task<ModuleDto?> GetModuleWithCoursAsync(Guid id);
        Task<IEnumerable<ModuleDto>> GetModulesBySprintIdAsync(Guid sprintId);
        Task<ModuleDto> CreateModuleAsync(CreateModuleDto createModuleDto);
        Task<ModuleDto?> UpdateModuleAsync(Guid id, UpdateModuleDto updateModuleDto);
        Task<bool> DeleteModuleAsync(Guid id);
    }
}

