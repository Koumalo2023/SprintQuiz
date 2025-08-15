using SprintQuiz.Api.DTOs;

namespace SprintQuiz.Api.Services
{
    public interface ISprintService
    {
        Task<IEnumerable<SprintDto>> GetAllSprintsAsync();
        Task<SprintDto?> GetSprintByIdAsync(Guid id);
        Task<SprintDto?> GetSprintWithModulesAsync(Guid id);
        Task<SprintDto> CreateSprintAsync(CreateSprintDto createSprintDto);
        Task<SprintDto?> UpdateSprintAsync(Guid id, UpdateSprintDto updateSprintDto);
        Task<bool> DeleteSprintAsync(Guid id);
    }
}

